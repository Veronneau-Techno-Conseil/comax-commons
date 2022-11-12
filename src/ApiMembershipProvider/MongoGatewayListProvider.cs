using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Orleans.Configuration;
using Orleans.Messaging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.ApiMembershipProvider
{

    public sealed class MongoGatewayListProvider : IGatewayListProvider
    {
        private readonly IMongoClient mongoClient;
        private readonly ILogger<MongoGatewayListProvider> logger;
        private readonly MongoDBOptions options;
        private readonly string clusterId;
        private IMongoMembershipCollection gatewaysCollection;

        /// <inheritdoc />
        public bool IsUpdatable { get; } = true;

        /// <inheritdoc />
        public TimeSpan MaxStaleness { get; }

        public MongoGatewayListProvider(
            IMongoClientFactory mongoClientFactory,
            ILogger<MongoGatewayListProvider> logger,
            IOptions<ClusterOptions> clusterOptions,
            IOptions<GatewayOptions> gatewayOptions,
            IOptions<MongoDBOptions> options)
        {
            this.mongoClient = mongoClientFactory.Create(options.Value, "Membership");
            this.logger = logger;
            this.options = options.Value;
            this.clusterId = clusterOptions.Value.ClusterId;
            this.MaxStaleness = gatewayOptions.Value.GatewayListRefreshPeriod;
        }

        /// <inheritdoc />
        public Task InitializeGatewayListProvider()
        {
            CreateCollection();

            return Task.CompletedTask;
        }

        private void CreateCollection()
        {
            gatewaysCollection = new SingleMembershipCollection(
                        mongoClient,
                        options.DatabaseName,
                        options.CollectionPrefix,
                        options.CreateShardKeyForCosmos,null);
        }

        /// <inheritdoc />
        public Task<IList<Uri>> GetGateways()
        {
            return DoAndLog(nameof(GetGateways), () =>
            {
                return gatewaysCollection.GetGateways(clusterId);
            });
        }

        private async Task<T> DoAndLog<T>(string actionName, Func<Task<T>> action)
        {
            logger.LogDebug($"{nameof(MongoGatewayListProvider)}.{actionName} called.");

            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                logger.LogWarning((int)MongoProviderErrorCode.MembershipTable_Operations, ex, $"{nameof(MongoGatewayListProvider)}.{actionName} failed. Exception={ex.Message}");

                throw;
            }
        }
    }

}
