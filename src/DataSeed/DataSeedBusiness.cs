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

namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeedBusiness
    {
        private DataSeedRepo? _dataSeedRepo;
        private IComaxGrainFactory _comaxGrainFactory;
        private readonly string _grainKey;

        public DataSeedBusiness(IComaxGrainFactory comaxGrainFactory, string grainKey)
        {
            _comaxGrainFactory = comaxGrainFactory;
            _grainKey = grainKey;
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

        public async Task<List<DataSeedObject>> BuildIndexes(DataSeedResult dsResult)
        {
            var index = new DataIndex();
            var dsDataList = new List<DataSeedObject>();
            // upload indexes into DataSeedGrain
            for(int i=0; i < dsResult.DS.Count; i++)
            {
                var dsData = new DataSeedObject();
                var identifier = $"{_grainKey}-index";
                var indexStorage = _comaxGrainFactory.GetGrain<IDataSeed>(Guid.Parse(identifier));
                dsData.Id = indexStorage.GetPrimaryKey();
                dsData.Index = JObject.FromObject(dsResult.DS[i]);
                dsDataList.Add(dsData);
            }
            return dsDataList;
        }
        public async Task<List<DataChunkObject>> BuildRows(DataChunkResult dcResult)
        {
            //upload the rows into DataChunkGrain
            var dcDataList = new List<DataChunkObject>();
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
                dcDataList.Add(dcData);
            }
            return dcDataList;
        }
    }
}
