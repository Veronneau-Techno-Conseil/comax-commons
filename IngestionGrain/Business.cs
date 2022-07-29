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

        public async Task<IngestionState> Run()
        {
            if (_ingestionState == IngestionState.Started)
            {
                return await Task.FromResult(IngestionState.InProcess);
            }

            return await Task.Run(async () =>
            {
                try
                {
                    _ingestionState = IngestionState.Started;

                    var dataSource = _grainFactory.GetGrain<IDatasource>(_grainKey);
                    var state = await dataSource.GetConfig();
                    var sourfceConfig = new SourceConfig { Configurations = state.Configurations, DataSourceType = state.DataSourceType };

                    var result = await _importer.Import(sourfceConfig, state.Fields);

                    var index = new DataIndex();


                    // save rows
                    for (int i = 0; i < result.Rows.Count; i++)
                    {
                        var identifier = $"{_grainKey}-data-{i}";
                        index.Index.Add(new DataIndexItem { Id = identifier });
                        var rowStorage = _grainFactory.GetGrain<IStorageGrain>(identifier);
                        await rowStorage.SaveData(result.Rows[i]);
                    }

                    var storage = _grainFactory.GetGrain<IStorageGrain>($"{_grainKey}-index");

                    // save errors
                    for (int i = 0; i < result.Errors.Count; i++)
                    {
                        var identifier = $"{_grainKey}-err-{i}";
                        index.Index.Add(new DataIndexItem { Id = identifier });
                        var rowStorage = _grainFactory.GetGrain<IStorageGrain>(identifier);
                        await rowStorage.SaveData(result.Errors[i].Item1);
                    }

                    var temp = JObject.FromObject(index);
                    await storage.SaveData(temp);

                    await _repo.AddHistory(new() { CreateDateTime = DateTime.UtcNow, IsSuccessful = true });
                }
                catch (Exception ex)
                {
                    await _repo.AddHistory(new() { CreateDateTime = DateTime.UtcNow, IsSuccessful = true, Exception = ex });
                }
                finally
                {
                    _ingestionState = IngestionState.Completed;
                }

                return _ingestionState;
            });

        }
    }
}
