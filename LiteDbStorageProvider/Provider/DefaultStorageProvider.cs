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
    public class DefaultStorageProvider : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {

        private readonly string _name;
        private LiteDatabase _db;
        private readonly ILogger<DefaultStorageProvider> _logger;
        private readonly LiteDbConfig _cfg;

        private ISerializationProvider _serializationProvider;
        private readonly IServiceProvider _serviceProvider;

        public DefaultStorageProvider(string name, ILogger<DefaultStorageProvider> logger, LiteDbConfig liteDbConfig, IServiceProvider serviceProvider)
        {
            _name = name;
            _logger = logger;
            _cfg = liteDbConfig;
            _serviceProvider = serviceProvider;
        }

        public static string GetBlobName(string grainType, GrainReference grainId)
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
            var collection = await CreateLiteCollection(grainState.Type.Name);
            var grain = collection.Query().Where(x => x["ETag"].AsString == blobName).FirstOrDefault();

            if (grain != null)
            {
                collection.Delete(grain);
            }
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            return Task.Run(async () =>
            {
                var blobName = GetBlobName(grainType, grainReference);
                var collection = await CreateLiteCollection(grainState.Type.Name);
                var grain = collection.Query().Where(x => x["ETag"].AsString == blobName).FirstOrDefault();
                if (grain == null)
                    return;

                dynamic obj = BsonMapper.Global.Deserialize(typeof(GrainStorageModel<>).MakeGenericType(grainState.Type), grain);

                grainState.State = obj.Contents;
                grainState.ETag = obj.ETag;
            });
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            return Task.Run(async () =>
            {
                var blobName = GetBlobName(grainType, grainReference);
                var collection = await CreateLiteCollection(grainState.Type.Name);
                var grain = collection.Query().Where(x => x["ETag"].AsString == blobName).FirstOrDefault();
                object obj;
                if (grain == null)
                {
                    obj = Activator.CreateInstance(typeof(GrainStorageModel<>).MakeGenericType(grainState.Type));
                    Assign(obj, "Id", LiteDB.ObjectId.NewObjectId());
                }
                else
                {
                    obj = BsonMapper.Global.Deserialize(typeof(GrainStorageModel<>).MakeGenericType(grainState.Type), grain);
                }
                Assign(obj, "ETag", blobName);
                Assign(obj, "Contents", grainState.State);

                var doc = BsonMapper.Global.Serialize(typeof(GrainStorageModel<>).MakeGenericType(grainState.Type), obj);
                collection.Upsert(doc.AsDocument);
            });
        }

        private void Assign(object o, string property, object value)
        {
            o.GetType().GetProperty(property).SetValue(o, value, null);
        }

        private async Task<ILiteCollection<BsonDocument>> CreateLiteCollection(string name)
        {
            return await Task.Run(() =>
            {
                var _grains = _db.GetCollection(name, BsonAutoId.ObjectId);
                _grains.EnsureIndex(BsonExpression.Create("LOWER($.ETag)"), unique: true);
                return _grains;
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

                _db = Common.GetOrAdd(_cfg.FileName);

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
