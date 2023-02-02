using Microsoft.VisualBasic;
using Orleans.Runtime;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Comax.Commons.Orchestrator.MembershipProvider.Models;
using MongoDB.Bson;

namespace Comax.Commons.Orchestrator.MongoDbMembershipStorage
{
    public sealed class MongoMembershipStorage : IMembershipStorage
    {
        private const string CollectionFormat = "{0}Set";

        protected static readonly UpdateOptions Upsert = new UpdateOptions { IsUpsert = true };
        protected static readonly ReplaceOptions UpsertReplace = new ReplaceOptions { IsUpsert = true };
        protected static readonly SortDefinitionBuilder<Cluster> Sort = Builders<Cluster>.Sort;
        protected static readonly UpdateDefinitionBuilder<Cluster> Update = Builders<Cluster>.Update;
        protected static readonly FilterDefinitionBuilder<Cluster> Filter = Builders<Cluster>.Filter;
        protected static readonly IndexKeysDefinitionBuilder<Cluster> Index = Builders<Cluster>.IndexKeys;
        protected static readonly ProjectionDefinitionBuilder<Cluster> Project = Builders<Cluster>.Projection;

        private readonly IMongoDatabase mongoDatabase;
        private readonly IMongoClient mongoClient;
        private readonly Lazy<IMongoCollection<Cluster>> mongoCollection;
        private readonly bool createShardKey;

        protected IMongoCollection<Cluster> Collection
        {
            get { return mongoCollection.Value; }
        }

        protected IMongoDatabase Database
        {
            get { return mongoDatabase; }
        }

        public IMongoClient Client
        {
            get { return mongoClient; }
        }

        private static readonly TableVersion NotFound = new TableVersion(0, "0");
        private readonly string collectionPrefix;

        public MongoMembershipStorage(IMongoClientFactory mongoClientFactory, IOptions<MongoDBOptions> options)
        {
            this.collectionPrefix = options.Value.CollectionPrefix;
            this.mongoClient = mongoClientFactory.Create(options.Value, "Membership");

            mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName, new MongoDatabaseSettings
            {
                ReadPreference = ReadPreference.SecondaryPreferred,
                ReadConcern = ReadConcern.Local,
                WriteConcern = WriteConcern.Acknowledged
            });
            mongoCollection = CreateCollection(options.Value.CollectionConfigurator);

            this.createShardKey = options.Value.CreateShardKeyForCosmos;
        }

        protected string CollectionName()
        {
            return $"{collectionPrefix}OrleansMembershipSingle";
        }

        public async Task CleanupDefunctSiloEntries(string deploymentId, DateTimeOffset beforeDate)
        {
            var deployment = await Collection.Find(x => x.DeploymentId == deploymentId).FirstOrDefaultAsync();

            var updates = new List<UpdateDefinition<Cluster>>();

            foreach (var kvp in deployment.Members)
            {
                var member = kvp.Value;

                if (member.Status != (int)SiloStatus.Active && member.Timestamp < beforeDate)
                {
                    updates.Add(Update.Unset($"Members.{kvp.Key}"));
                }
            }

            if (updates.Count > 0)
            {
                var update = Update.Combine(updates);

                await Collection.UpdateOneAsync(x => x.DeploymentId == deploymentId, update);
            }
        }

        public Task DeleteMembershipTableEntries(string deploymentId)
        {
            return Collection.DeleteOneAsync(x => x.DeploymentId == deploymentId);
        }

        public async Task<IList<Uri>> GetGateways(string deploymentId)
        {
            var deployment = await Collection.Find(x => x.DeploymentId == deploymentId).FirstOrDefaultAsync();

            if (deployment == null)
            {
                return new List<Uri>();
            }

            return deployment.Members.Values.Where(x => x.Status == (int)SiloStatus.Active && x.ProxyPort > 0).Select(x => x.ToGatewayUri()).ToList();
        }

        public async Task<MembershipTableData> ReadAll(string deploymentId)
        {
            var deployment = await Collection.Find(x => x.DeploymentId == deploymentId).FirstOrDefaultAsync();

            if (deployment == null)
            {
                return new MembershipTableData(NotFound);
            }

            return deployment.ToTable();
        }

        public async Task<MembershipTableData> ReadRow(string deploymentId, SiloAddress address)
        {
            var deployment = await Collection.Find(x => x.DeploymentId == deploymentId).FirstOrDefaultAsync();

            if (deployment == null)
            {
                return new MembershipTableData(NotFound);
            }

            return deployment.ToTable(BuildKey(address));
        }

        public async Task UpdateIAmAlive(string deploymentId, SiloAddress address, DateTime iAmAliveTime)
        {
            await Collection.UpdateOneAsync(x => x.DeploymentId == deploymentId,
                Update.Set($"Members.{BuildKey(address)}.IAmAliveTime", LogFormatter.PrintDate(iAmAliveTime)));
        }

        public async Task<bool> UpsertRow(string deploymentId, MembershipEntry entry, string etag, TableVersion tableVersion)
        {
            try
            {
                var subDocument = Member.Create(entry);

                var memberKey = $"Members.{BuildKey(entry.SiloAddress)}";

                var etagCheck =
                    etag == null ?
                        Filter.Not(Filter.Exists(memberKey)) :
                        Filter.Eq($"{memberKey}.Etag", etag);

                var result = await Collection.UpdateOneAsync(
                    Filter.And(
                        Filter.Eq(x => x.DeploymentId, deploymentId),
                        Filter.Eq(x => x.VersionEtag, tableVersion.VersionEtag),
                        etagCheck),
                    Update
                        .Set(memberKey, subDocument)
                        .Set(x => x.Version, tableVersion.Version)
                        .Set(x => x.VersionEtag, Guid.NewGuid().ToString()),
                    Upsert);

                return true;
            }
            catch (MongoException ex)
            {
                if (ex.IsDuplicateKey())
                {
                    return false;
                }
                throw;
            }
        }

        private static string BuildKey(SiloAddress address)
        {
            return address.ToParsableString().Replace('.', '_');
        }

        protected MongoCollectionSettings CollectionSettings()
        {
            return new MongoCollectionSettings();
        }

        private Lazy<IMongoCollection<Cluster>> CreateCollection(Action<MongoCollectionSettings> collectionConfigurator)
        {
            return new Lazy<IMongoCollection<Cluster>>(() =>
            {
                var collectionFilter = new ListCollectionNamesOptions
                {
                    Filter = Builders<BsonDocument>.Filter.Eq("name", CollectionName())
                };

                var collectionSettings = CollectionSettings() ?? new MongoCollectionSettings();

                collectionConfigurator?.Invoke(collectionSettings);

                var databaseCollection = mongoDatabase.GetCollection<Cluster>(
                    CollectionName(),
                    collectionSettings);

                if (this.createShardKey)
                {
                    try
                    {
                        Database.RunCommand<BsonDocument>(new BsonDocument
                        {
                            ["key"] = new BsonDocument
                            {
                                ["_id"] = "hashed"
                            },
                            ["shardCollection"] = $"{mongoDatabase.DatabaseNamespace.DatabaseName}.{CollectionName()}"
                        });
                    }
                    catch (MongoException)
                    {
                        // Shared key probably created already.
                    }
                }

                SetupCollection(databaseCollection);

                return databaseCollection;
            });
        }

        protected void SetupCollection(IMongoCollection<Cluster> collection)
        {
        }

        public Task Init(string deploymentId)
        {
            return Task.CompletedTask;
        }
    }
}
