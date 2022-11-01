using Comax.Commons.StorageProvider.Hosting;
using Comax.Commons.StorageProvider.Serialization;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Orleans;
using System.Linq;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Comax.Commons.StorageProvider.Models;

namespace Comax.Commons.StorageProvider
{
    internal class MongoDbStorageProvider : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {

        private readonly string _name;
        MongoDB.Driver.IMongoClient _client;
        private IMongoDatabase _mongoDatabase;
        private IMongoCollection<GrainStorageModel> _mongoCollection;
        private readonly ILogger<MongoDbStorageProvider> _logger;
        private ISerializationProvider _serializationProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly MongodbConfig _mongodbConfig;

        public MongoDbStorageProvider(string name, ILogger<MongoDbStorageProvider> logger, MongodbConfig mongoDbConfig, IServiceProvider serviceProvider)
        {
            _name = name;
            _logger = logger;
            _mongodbConfig = mongoDbConfig;
            _serviceProvider = serviceProvider;
            _client = new MongoDB.Driver.MongoClient(_mongodbConfig.ConnectionString);
        }

        private static string GetBlobName(string grainType, GrainReference grainId)
        {
            return string.Format("{0}-{1}.json", grainType, grainId.ToKeyString());
        }

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<MongoDbStorageProvider>(_name),
                                    ServiceLifecycleStage.RuntimeInitialize, Init);
        }
        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainReference);

            var res = await _mongoCollection.DeleteOneAsync(Builders<GrainStorageModel>.Filter.Eq(x => x.ETag, blobName));
            
            //TODO: Logging
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            var blobName = GetBlobName(grainType, grainReference);
            var blob = await (await _mongoCollection.FindAsync(Builders<GrainStorageModel>.Filter.Eq(x => x.ETag, blobName))).FirstOrDefaultAsync();

            if (blob == null)
                return;

            var contents = blob.Contents;
            grainState.State = _serializationProvider.Deserialize(blob.Contents, grainState.Type);
            grainState.ETag = blob.ETag;
            //TODO: Logging
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var blobName = GetBlobName(grainType, grainReference);
            var contents = _serializationProvider.Serialize(grainState.State);
            var blob = await (await _mongoCollection.FindAsync(Builders<GrainStorageModel>.Filter.Eq(x => x.ETag, blobName))).FirstOrDefaultAsync();

            if (blob == null)
            {
                blob = new GrainStorageModel() { ETag = blobName, Contents = contents };
                await _mongoCollection.InsertOneAsync(blob);
            }
            else
            {

                await _mongoCollection.UpdateOneAsync(
                    Builders<GrainStorageModel>.Filter.Eq(x => x.ETag, blobName),
                    Builders<GrainStorageModel>.Update.Set(x => x.Contents, contents));
            }
            //TODO: Logging
        }

        private async Task Init(CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation($"{this.GetType().Name} - initialize container {this._mongodbConfig.Collection}");

                _serializationProvider = _serviceProvider.GetServiceByName<ISerializationProvider>(_mongodbConfig.SerializationProvider);

                _mongoDatabase = _client.GetDatabase(_mongodbConfig.DatabaseName);

                if (!string.IsNullOrWhiteSpace(_mongodbConfig.SerializationConfig))
                {
                    _serializationProvider.Configure(_mongodbConfig.SerializationConfig);
                }


                var field = new StringFieldDefinition<GrainStorageModel>(_mongodbConfig.Collection);
                var indexDefinition = new IndexKeysDefinitionBuilder<GrainStorageModel>().Ascending(field);

                _mongoCollection = _mongoDatabase.GetCollection<GrainStorageModel>(_mongodbConfig.Collection);

                await _mongoCollection.Indexes
                    .CreateOneAsync(new CreateIndexModel<GrainStorageModel>(
                        Builders<GrainStorageModel>.IndexKeys.Descending(x => x.ETag), new CreateIndexOptions() { Unique = true }));


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
