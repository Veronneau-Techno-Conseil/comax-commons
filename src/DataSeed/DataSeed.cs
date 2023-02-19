using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;
using CommunAxiom.Commons.CommonsShared.Contracts.DataSeed;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeed : Grain, IDataSeed
    {
        private readonly IPersistentState<DataIndex> _dataSeedState;
        private readonly IServiceProvider _serviceProvider;
        public DataSeed([PersistentState("dataSeedGrain")] IPersistentState<DataIndex> DataSeedObject, IServiceProvider serviceProvider)
        {
            _dataSeedState = DataSeedObject;
            _serviceProvider = serviceProvider;
        }
        public DataSeedBusiness _dataSeedBusiness;

        public override Task OnActivateAsync()
        {
            _dataSeedBusiness = new DataSeedBusiness(this.GetPrimaryKey(), 
                new GrainFactory(this.GrainFactory, this.GetStreamProvider), 
                _serviceProvider.GetService<ILogger<DataSeed>>());
            _dataSeedBusiness.Init(_dataSeedState);
            return base.OnActivateAsync();
        }

        public async Task RetrieveData(Guid streamId)
        {
            await _dataSeedBusiness.StreamDataFromStorage(streamId);
            
        }

        public async Task SendIndex(DataIndex value)
        {
            await _dataSeedBusiness.BuildIndexes(value);
        }

        public async Task UploadData(DataChunkResult value)
        {
            //await _dataSeedBusiness.BuildRows(value);
        }

        public Task Validate()
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> SetUploadStream(Guid id)
        {
            var ix = await _dataSeedBusiness.GetIndex();
            if (ix == null || ix.Id == Guid.Empty)
                return new OperationResult
                {
                    IsError = true,
                    Error = OperationResult.ERR_NOT_FOUND,
                    Detail = "The index needs to be saved before uploading data"
                };

            return await _dataSeedBusiness.ListenForUpload(id);
        }

        public async Task<DataIndex> GetIndex()
        {
            return await _dataSeedBusiness.GetIndex();
        }
    }
}
