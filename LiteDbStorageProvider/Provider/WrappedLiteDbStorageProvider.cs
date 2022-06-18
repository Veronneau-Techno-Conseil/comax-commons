using Comax.Commons.StorageProvider.Hosting;
using Comax.Commons.StorageProvider.Models;
using Comax.Commons.StorageProvider.Serialization;
using LiteDB;
using Microsoft.Extensions.Logging;
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
        private LiteDatabase _db;
        private readonly ILogger<WrappedLiteDbStorageProvider> _logger;
        private readonly LiteDbConfig _cfg;
        private ILiteCollection<GrainStorageModel> _grains;
        private ISerializationProvider _serializationProvider;
        private readonly IServiceProvider _serviceProvider;

        public WrappedLiteDbStorageProvider(string name, ILogger<WrappedLiteDbStorageProvider> logger, LiteDbConfig liteDbConfig, IServiceProvider serviceProvider)
        {
            _name = name;
            _logger = logger;
            _cfg = liteDbConfig;
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
        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainReference);
            var blob = _grains.Query().Where(x => x.ETag == blobName).FirstOrDefault();
            if (blob != null)
            {
                _grains.Delete(blob.Id);
            }
            return Task.CompletedTask;
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            return Task.Run(() =>
            {
                var blobName = GetBlobName(grainType, grainReference);
                var blob = _grains.Query().Where(x => x.ETag == blobName).FirstOrDefault();
                if (blob == null)
                    return;
                grainState.State = _serializationProvider.Deserialize(blob.Contents, grainState.Type);
                grainState.ETag = blob.ETag;
            });
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            return Task.Run(() =>
            {
                var blobName = GetBlobName(grainType, grainReference);
                var contents = _serializationProvider.Serialize(grainState.State);
                var blob = _grains.Query().Where(x => x.ETag == blobName).FirstOrDefault();
                if (blob == null)
                    blob = new GrainStorageModel() { ETag = blobName, Contents = contents };
                else
                    blob.Contents = contents;

                _grains.Upsert(blob);
            });
        }

        private async Task Init(CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();
            if (_db != null)
                return;
            try
            {
                _logger.LogInformation($"LiteDbStorageAdapter - initialize container grainstate");

                _serializationProvider = _serviceProvider.GetServiceByName<ISerializationProvider>(_cfg.SerializationProvider);
                if (!string.IsNullOrWhiteSpace(_cfg.SerializationConfig))
                {
                    _serializationProvider.Configure(_cfg.SerializationConfig);
                }

                _db = Common.GetOrAdd(_name);
                await Task.Run(() => _grains = _db.GetCollection<GrainStorageModel>("grains"));
                _grains.EnsureIndex(x => x.ETag, unique: true);
                
                stopWatch.Stop();
                _logger.LogInformation($"Initializing provider {_name} of type {this.GetType().Name} in stage { ServiceLifecycleStage.RuntimeInitialize } took {stopWatch.ElapsedMilliseconds} Milliseconds.");
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
