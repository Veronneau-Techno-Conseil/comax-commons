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
using Orleans.Configuration;
using CommunAxiom.Commons.Shared;

namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeedBusiness : IAsyncObserver<DataChunkObject>
    {
        private readonly Guid _id;
        private DataSeedRepo? _dataSeedRepo;
        private IComaxGrainFactory _comaxGrainFactory;
        private IAsyncStream<DataChunkObject> _stream;
        private readonly ILogger _logger;

        private bool _isUploading = false;
        private Guid _streamId = Guid.Empty;
        private StreamSubscriptionHandle<DataChunkObject> _uploadStreamSubscription;

        public DataSeedBusiness(Guid id, IComaxGrainFactory comaxGrainFactory, ILogger logger)
        {
            _comaxGrainFactory = comaxGrainFactory;
            _logger = logger;
            _id = id;
        }

        public void Init(IPersistentState<DataIndex> dsState)
        {
            _dataSeedRepo = new DataSeedRepo(dsState);
        }

        public async Task<Guid> FetchGuid(string dsUri)
        {
            var grain = _comaxGrainFactory.GetGrain<IUriRegistry>(dsUri);
            var dsGuid = await grain.GetOrCreate();
            return dsGuid;
        }

        public async Task<OperationResult> StreamDataFromStorage(Guid streamId)
        {
            //instanciate the stream
            var sp = _comaxGrainFactory.GetStreamProvider(OrleansConstants.Streams.DefaultStream);
            _stream = sp.GetStream<DataChunkObject>(streamId, DataSeedConstants.DATA_SEED_NAMESPACE);
            //retrieve index from state
            var ix = await GetIndex();
            if (ix == null || ix.Id == Guid.Empty)
            {
                return new OperationResult
                {
                    IsError = true,
                    Error = OperationResult.ERR_NOT_FOUND,
                    Detail = "Index is empty"
                };
            }
            //iterate through index items
            foreach (var item in ix.Index)
            {
                //call SendRow on index items passing along streamid not stream
                await SendRow(item, streamId);
            }
            return new OperationResult();
        }

        public async Task<DataIndex> GetIndex()
        {
            return await _dataSeedRepo.Fetch();
        }

        public async Task BuildIndexes(DataIndex dsResult)
        {
            await _dataSeedRepo.Save(dsResult);
        }
        public async Task SendRow(DataIndexItem ixItem, Guid streamId)
        {
            // using datachunk grain with ixItemId
            var grain = _comaxGrainFactory.GetGrain<IDataChunk>(ixItem.Id);
            // retrieve data and send through stream using onnext
            var data = await grain.GetData();
            _uploadStreamSubscription = await _stream.SubscribeAsync(this);
            await _stream.OnNextAsync(data);
            await _stream.OnCompletedAsync();
        }

        public async Task OnCompletedAsync()
        {
            try
            {
                await _uploadStreamSubscription.UnsubscribeAsync();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _uploadStreamSubscription = null;
                _isUploading = false;
            }
        }

        public Task OnErrorAsync(Exception ex)
        {
            _logger.LogError("Error received from upload stream", ex);
            return Task.CompletedTask;
        }

        public async Task<OperationResult> ListenForUpload(Guid id)
        {
            if (_isUploading)
            {
                return new OperationResult
                {
                    IsError = true,
                    Error = OperationResult.ERR_INVALID_OPERATION,
                    Detail = "Upload in progress"
                };
            }

            var sp = _comaxGrainFactory.GetStreamProvider(OrleansConstants.Streams.DefaultStream);
            var stream = sp.GetStream<DataChunkObject>(id, DataSeedConstants.DATA_SEED_NAMESPACE);
            this._uploadStreamSubscription = await stream.SubscribeAsync(this);
            _isUploading = true;
            return new OperationResult();
        }

        public async Task OnNextAsync(DataChunkObject item, StreamSequenceToken token = null)
        {
            var dc = this._comaxGrainFactory.GetGrain<IDataChunk>(item.IdDataChunk.Id.ToString());
            await dc.SaveData(item);
        }
    }
}
