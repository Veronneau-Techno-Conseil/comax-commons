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

        public void Init(IPersistentState<DataSeedState> dsState)
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
            DataSeedResult result = new DataSeedResult(data);
            return result;
        }

        public async Task BuildIndexesAndRows(DataSeedResult result)
        {
            var index = new DataIndex();
            for(int i=0; i < result.Rows.Count; i++)
            {
                var identifier = $"{_grainKey}-data-{i}";
                index.Index.Add(new DataIndexItem { Id= identifier });
                var rowStorage = result.Rows[i];
            }
            //save indexes into DataSeedGrain
            //var temp;            
            //await _dataSeedRepo.Save(temp);
            //upload the rows into DataChunkGrain
        }


        //read and deserialize indexes
        //build DataIndex object, save to state
        //send index to DataSeed, send row to DataChunk

    }
}
