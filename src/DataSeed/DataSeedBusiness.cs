using CommunAxiom.Commons.CommonsShared.Contracts.DataSeed;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using CommunAxiom.Commons.Orleans;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Comax.Commons.Orchestrator.DataChunkGrain;
using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;
using Orleans;
using Orleans.Streams;
using Microsoft.Extensions.Logging;

namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeedBusiness
    {
        private DataSeedRepo? _dataSeedRepo;
        private IComaxGrainFactory _comaxGrainFactory;
        private readonly string _grainKey;
        private IAsyncStream<DataChunkObject> _stream;
        private IStreamProvider _streamProvider;
        private readonly ILogger _logger;

        public DataSeedBusiness(IComaxGrainFactory comaxGrainFactory, string grainKey, IStreamProvider streamProvider, ILogger logger)
        {
            _comaxGrainFactory = comaxGrainFactory;
            _grainKey = grainKey;
            _streamProvider = streamProvider;
            _logger = logger;
        }

        public void Init(IPersistentState<DataSeedObject> dsState)
        {
            _dataSeedRepo = new DataSeedRepo(dsState);
        }

        public async Task<Guid> FetchGuid(string dsUri)
        {
            var grain = _comaxGrainFactory.GetGrain<IUriRegistry>(dsUri);
            var dsGuid = await grain.GetOrCreate();
            return dsGuid;
        }

        public async Task<DataSeedResult> GetDataFromStorage(string dsUri)
        {
            var storageGuid = FetchGuid(dsUri).ToString();
            var grain = _comaxGrainFactory.GetGrain<IStorageGrain>(storageGuid);
            var data = await grain.GetData();
            DataSeedResult result = new DataSeedResult(new DataSeedObject(Guid.Parse(storageGuid), data));
            return result;
        }

        public async Task BuildIndexes(DataSeedResult dsResult)
        {
            var index = new DataIndex();
            // upload indexes into DataSeedGrain
            for(int i=0; i < dsResult.DS.Count; i++)
            {
                var dsData = new DataSeedObject();
                var identifier = $"{_grainKey}-index";
                var indexStorage = _comaxGrainFactory.GetGrain<IDataSeed>(Guid.Parse(identifier));
                dsData.Id = indexStorage.GetPrimaryKey();
                dsData.Index = JObject.FromObject(dsResult.DS[i]);
                await _dataSeedRepo.Save(dsData);
            }
            
        }
        public async Task BuildRows(DataChunkResult dcResult)
        {
            //upload the rows into DataChunkGrain
            for (int i = 0; i < dcResult.DC.Count; i++)
            {
                var dcData = new DataChunkObject();
                var identifier = $"{_grainKey}-data-{i}";
                var dataStorage = _comaxGrainFactory.GetGrain<IDataChunk>(identifier);
                dcData.Id = dataStorage.GetPrimaryKey();
                identifier = $"{_grainKey}-index";
                var indexStorage = _comaxGrainFactory.GetGrain<IDataSeed>(Guid.Parse(identifier));
                dcData.IdDataSeed = indexStorage.GetPrimaryKey();
                dcData.Data = JObject.FromObject(dcResult.DC[i]);
                _stream = _streamProvider.GetStream<DataChunkObject>(dcData.Id, OrleansConstants.StreamNamespaces.DefaultNamespace);
                await _stream.OnNextAsync(dcData);
            }
        }

        public Task OnNextAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnCompletedAsync()
        {     
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            throw ex;
        }

        
    }
}
