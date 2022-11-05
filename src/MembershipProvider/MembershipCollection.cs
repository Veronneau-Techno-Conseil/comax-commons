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

namespace Comax.Commons.Orchestrator.MembershipProvider
{
    public interface IMongoMembershipCollection
    {
        Task CleanupDefunctSiloEntries(string deploymentId, DateTimeOffset beforeDate);

        Task DeleteMembershipTableEntries(string deploymentId);

        Task<IList<Uri>> GetGateways(string deploymentId);

        Task<MembershipTableData> ReadAll(string deploymentId);

        Task<MembershipTableData> ReadRow(string deploymentId, SiloAddress address);

        Task UpdateIAmAlive(string deploymentId, SiloAddress address, DateTime iAmAliveTime);

        Task<bool> UpsertRow(string deploymentId, MembershipEntry entry, string etag, TableVersion tableVersion);
    }
    public sealed class SingleMembershipCollection : CollectionBase<DeploymentDocument>, IMongoMembershipCollection
    {
        private static readonly TableVersion NotFound = new TableVersion(0, "0");
        private readonly string collectionPrefix;

        public SingleMembershipCollection(IMongoClient mongoClient, string databaseName, string collectionPrefix, bool createShardKey,
            Action<MongoCollectionSettings> collectionConfigurator)
            : base(mongoClient, databaseName, collectionConfigurator, createShardKey)
        {
            this.collectionPrefix = collectionPrefix;
        }

        protected override string CollectionName()
        {
            return $"{collectionPrefix}OrleansMembershipSingle";
        }

        public async Task CleanupDefunctSiloEntries(string deploymentId, DateTimeOffset beforeDate)
        {
            var deployment = await Collection.Find(x => x.DeploymentId == deploymentId).FirstOrDefaultAsync();

            var updates = new List<UpdateDefinition<DeploymentDocument>>();

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
                Update
                    .Set($"Members.{BuildKey(address)}.IAmAliveTime", LogFormatter.PrintDate(iAmAliveTime)));
        }

        public async Task<bool> UpsertRow(string deploymentId, MembershipEntry entry, string etag, TableVersion tableVersion)
        {
            try
            {
                var subDocument = MembershipBase.Create<DeploymentMembership>(entry);

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
                        .Set(x => x.VersionEtag, EtagHelper.CreateNew()),
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


    }

    public static class MongoExtensions
    {
        public static bool IsDuplicateKey(this MongoException ex)
        {
            if (ex is MongoCommandException c && c.Code == 11000)
            {
                return true;
            }
            if (ex is MongoWriteException w && w.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return true;
            }
            return false;
        }

        public static IMongoClient Create(this IMongoClientFactory mongoClientFactory, MongoDBOptions options, string defaultName)
        {
            var name = options.ClientName;

            if (string.IsNullOrWhiteSpace(name))
            {
                name = defaultName;
            }

            return mongoClientFactory.Create(name);
        }
    }
    public interface IMongoClientFactory
    {
        IMongoClient Create(string name);
    }

    /// <summary>
    /// Options to configure MongoDB for Orleans
    /// </summary>
    public class MongoDBOptions
    {
        /// <summary>
        /// Database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The mongo client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// The collection prefix.
        /// </summary>
        public string CollectionPrefix { get; set; }

        /// <summary>
        /// True, to create a shard key when using with cosmos db.
        /// </summary>
        public bool CreateShardKeyForCosmos { get; set; }

        /// <summary>
        /// The collection configurator.
        /// </summary>
        public Action<MongoCollectionSettings> CollectionConfigurator { get; set; }

        internal virtual void Validate(string name = null)
        {
            var typeName = GetType().Name;

            if (!string.IsNullOrWhiteSpace(typeName))
            {
                typeName = $"{typeName} for {name}";
            }

            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                throw new OrleansConfigurationException($"Invalid {typeName} values for {nameof(DatabaseName)}. {nameof(DatabaseName)} is required.");
            }
        }
    }

    public sealed class MongoMembershipTable : IMembershipTable
    {
        private readonly IMongoClient mongoClient;
        private readonly ILogger<MongoMembershipTable> logger;
        private readonly MongoDBOptions options;
        private readonly string clusterId;
        private IMongoMembershipCollection membershipCollection;

        public MongoMembershipTable(
            IMongoClientFactory mongoClientFactory,
            ILogger<MongoMembershipTable> logger,
            IOptions<ClusterOptions> clusterOptions,
            IOptions<MongoDBOptions> options)
        {
            this.mongoClient = mongoClientFactory.Create(options.Value, "Membership");
            this.logger = logger;
            this.options = options.Value;
            this.clusterId = clusterOptions.Value.ClusterId;
        }

        /// <inheritdoc />
        public Task InitializeMembershipTable(bool tryInitTableVersion)
        {
            membershipCollection = new SingleMembershipCollection(
                        mongoClient,
                        options.DatabaseName,
                        options.CollectionPrefix, 
                        options.CreateShardKeyForCosmos, 
                        options.CollectionConfigurator);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeleteMembershipTableEntries(string deploymentId)
        {
            return DoAndLog(nameof(DeleteMembershipTableEntries), () =>
            {
                return membershipCollection.DeleteMembershipTableEntries(deploymentId);
            });
        }

        /// <inheritdoc />
        public Task<MembershipTableData> ReadRow(SiloAddress key)
        {
            return DoAndLog(nameof(ReadRow), () =>
            {
                return membershipCollection.ReadRow(clusterId, key);
            });
        }

        /// <inheritdoc />
        public Task<MembershipTableData> ReadAll()
        {
            return DoAndLog(nameof(ReadAll), () =>
            {
                return membershipCollection.ReadAll(clusterId);
            });
        }

        /// <inheritdoc />
        public Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion)
        {
            return DoAndLog(nameof(InsertRow), () =>
            {
                return membershipCollection.UpsertRow(clusterId, entry, null, tableVersion);
            });
        }

        /// <inheritdoc />
        public Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion)
        {
            return DoAndLog(nameof(UpdateRow), () =>
            {
                return membershipCollection.UpsertRow(clusterId, entry, etag, tableVersion);
            });
        }

        /// <inheritdoc />
        public Task CleanupDefunctSiloEntries(DateTimeOffset beforeDate)
        {
            return DoAndLog(nameof(CleanupDefunctSiloEntries), () =>
            {
                return membershipCollection.CleanupDefunctSiloEntries(clusterId, beforeDate);
            });
        }

        /// <inheritdoc />
        public Task UpdateIAmAlive(MembershipEntry entry)
        {
            return DoAndLog(nameof(UpdateRow), () =>
            {
                return membershipCollection.UpdateIAmAlive(clusterId,
                    entry.SiloAddress,
                    entry.IAmAliveTime);
            });
        }

        private Task DoAndLog(string actionName, Func<Task> action)
        {
            return DoAndLog(actionName, async () =>
            {
                await action();

                return true;
            });
        }

        private async Task<T> DoAndLog<T>(string actionName, Func<Task<T>> action)
        {
            logger.LogDebug($"{nameof(MongoMembershipTable)}.{actionName} called.");

            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                logger.LogError((int)MongoProviderErrorCode.MembershipTable_Operations, ex, $"{nameof(MongoMembershipTable)}.{actionName} failed. Exception={ex.Message}");

                throw;
            }
        }
    }
    internal enum MongoProviderErrorCode
    {
        ProvidersBase = 900000,

        GrainStorageOperations = ProvidersBase + 100,
        StorageProvider_Reading = GrainStorageOperations + 4,
        StorageProvider_Writing = GrainStorageOperations + 5,
        StorageProvider_Deleting = GrainStorageOperations + 6,

        MembershipTable_Operations = ProvidersBase + 200,

        StatisticsPublisher_Operations = ProvidersBase + 300,

        Reminders_Operations = ProvidersBase + 400
    }
}
