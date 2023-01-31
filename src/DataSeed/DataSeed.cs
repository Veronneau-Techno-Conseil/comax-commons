using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;
using CommunAxiom.Commons.CommonsShared.Contracts.DataSeed;
using CommunAxiom.Commons.Orleans;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeed : Grain, IDataSeed
    {
        private readonly IPersistentState<DataSeedObject> _dataSeedState;
        public DataSeed([PersistentState("dataSeedGrain")] IPersistentState<DataSeedObject> DataSeedObject)
        {
            _dataSeedState = DataSeedObject;
        }
        public DataSeedBusiness _dataSeedBusiness;

        public async Task RetrieveData(Guid id)
        {
            var data =await _dataSeedBusiness.GetDataFromStorage("dsUri");
            await SendIndex(data);
        }

        public async Task SendIndex(DataSeedResult value)
        {
            await _dataSeedBusiness.BuildIndexes(value);
        }

        public async Task UploadData(DataChunkResult value)
        {
            await _dataSeedBusiness.BuildRows(value);
        }

        public Task Validate()
        {
            throw new NotImplementedException();
        }

        public Task SetUploadStream(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
