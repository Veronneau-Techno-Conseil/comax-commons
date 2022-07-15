using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.IO;
using CommunAxiom.Commons.Ingestion;
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

        public Business(Importer importer, IGrainFactory grainFactory, string grainKey)
        {
            _importer = importer;
            _grainFactory = grainFactory;
            _grainKey = grainKey;
        }

        public void Init(IPersistentState<SourceState> sourceItem)
        {
            _repo = new Repo(sourceItem);
        }

        public Task<History> GetHistory()
        {
            throw new System.NotImplementedException();
        }

        public async Task Run()
        {
            try
            {
                var dataSource = _grainFactory.GetGrain<IDatasource>(_grainKey);
                var state = await dataSource.GetState();
                var sourfceConfig = new Ingestion.Configuration.SourceConfig { Configurations = state.Configurations, DataSourceType = state.DataSourceType };

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

            }
            catch (Exception ex)
            {
                // HACK: add history code here.
                // date: UtcNow / success / errors:  serilize exception details
            }

        }
    }
}
