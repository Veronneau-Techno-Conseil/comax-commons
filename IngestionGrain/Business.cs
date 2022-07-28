using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion;
using CommunAxiom.Commons.Orleans;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    public class Business
    {
        private Repo _repo;

        private readonly Importer _importer;
        private readonly IGrainFactory _grainFactory;
        private readonly string _grainKey;
        private IngestionState _ingestionState;

        public Business(Importer importer, IGrainFactory grainFactory, string grainKey)
        {
            _importer = importer;
            _grainFactory = grainFactory;
            _grainKey = grainKey;
        }

        public void Init(IPersistentState<IngestionHistory> history)
        {
            _repo = new Repo(history);
        }

        public Task<IngestionHistory> GetHistory()
        {
            return _repo.FetchHistory();
        }

        public async Task Run()
        {
            try
            {
                var dataSource = _grainFactory.GetGrain<IDatasource>(_grainKey);
                var state = await dataSource.GetState();
                var sourfceConfig = new SourceConfig { Configurations = state.Configurations, DataSourceType = state.DataSourceType };

                var result = await _importer.Import(sourfceConfig, state.Fields);

                var indexes = new List<string>();

                // save rows
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    var indetifier = $"{_grainKey}-{i}";
                    indexes.Add(indetifier);
                    var rowStorage = _grainFactory.GetGrain<IStorageGrain>(indetifier);
                    await rowStorage.SaveData(result.Rows[i]);
                }

                var storage = _grainFactory.GetGrain<IStorageGrain>($"{_grainKey}-index");

                var temp = JObject.FromObject(new { indexes = indexes });
                await storage.SaveData(temp);

                // save errors
                for (int i = 0; i < result.Errors.Count; i++)
                {
                    var indetifier = $"{_grainKey}-err-{i}";
                    indexes.Add(indetifier);
                    var rowStorage = _grainFactory.GetGrain<IStorageGrain>(indetifier);
                    await rowStorage.SaveData(result.Errors[i].Item1);
                }

                await _repo.AddHistory(new() { CreateDateTime = DateTime.UtcNow, IsSuccessful = true });
            }
            catch (Exception ex)
            {
                await _repo.AddHistory(new() { CreateDateTime = DateTime.UtcNow, IsSuccessful = true, Exception = ex });
            }

        }
    }
}
