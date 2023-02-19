using Comax.Commons.StorageProvider.Hosting;
using Comax.Commons.StorageProvider.Models;
using Comax.Commons.StorageProvider.Serialization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Comax.Commons.StorageProvider
{
    internal class WrappedLiteDbStorageProvider : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        private readonly string _name;

        private readonly ILogger<WrappedLiteDbStorageProvider> _logger;


        private ISerializationProvider _serializationProvider;
        private readonly GrainStorageClient _grainStorageClient;
        private readonly IServiceProvider _serviceProvider;
        public WrappedLiteDbStorageProvider(string name, ILogger<WrappedLiteDbStorageProvider> logger, GrainStorageClient grainStorageClient, IServiceProvider serviceProvider)
        {
            _name = name;
            _logger = logger;
            _grainStorageClient = grainStorageClient;
            _serviceProvider = serviceProvider;
        }

        private static string GetBlobName(string grainType, GrainReference grainId)
        {
            return string.Format("{0}-{1}.json", grainType, grainId.ToKeyString());
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<DefaultStorageProvider>(_name),
                                    ServiceLifecycleStage.RuntimeInitialize, Init);
        }
        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            var blobName = GetBlobName(grainType, grainReference);

            if (await _grainStorageClient.Any(grainType, blobName))
            {
                await _grainStorageClient.Delete(grainType, blobName);
            }

            grainState.RecordExists = false;
            grainState.State = null;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainReference);

            if (await _grainStorageClient.Any(grainType, blobName))
            {
                var jo = await _grainStorageClient.GetValue(grainType, blobName);
                var store = jo.ToObject<ByteValueStore>();

                grainState.State = _serializationProvider.Deserialize(store.Value, grainState.Type);
                grainState.RecordExists = true;
            }
            else
            {
                grainState.RecordExists = false;
            }

            grainState.ETag = blobName;
        }


        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            var blobName = GetBlobName(grainType, grainReference);

            if (grainState.State == null)
            {
                await ClearStateAsync(grainType, grainReference, grainState);
                return;
            }

            var contents = _serializationProvider.Serialize(grainState.State);
            ByteValueStore store = new ByteValueStore() { Value = contents };
            var jo = JObject.FromObject(store);

            await _grainStorageClient.UpsetValue(grainType, blobName, jo);
            grainState.RecordExists = true;
            grainState.ETag = blobName;

        }

        private async Task Init(CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();
            
            var _cfg = _serviceProvider.GetService<IOptionsMonitor<ApiStorageConfiguration>>();

            var cfg = _cfg.CurrentValue;

            try
            {
                _logger.LogInformation($"LiteDbStorageAdapter - initialize container grainstate");

                _serializationProvider = _serviceProvider.GetServiceByName<ISerializationProvider>(cfg.SerializationProvider);
                if (!string.IsNullOrWhiteSpace(cfg.SerializationConfig))
                {
                    _serializationProvider.Configure(cfg.SerializationConfig);
                }

                stopWatch.Stop();
                _logger.LogInformation($"Initializing provider {_name} of type {this.GetType().Name} in stage {ServiceLifecycleStage.RuntimeInitialize} took {stopWatch.ElapsedMilliseconds} Milliseconds.");
            }
            catch (Exception ex)
            {
                stopWatch.Stop();
                _logger.LogError((int)ErrorCode.Provider_ErrorFromInit, $"Initialization failed for provider {_name} of type {this.GetType().Name} in stage {ServiceLifecycleStage.RuntimeInitialize} in {stopWatch.ElapsedMilliseconds} Milliseconds.", ex);
                throw;
            }
        }
    }
}
