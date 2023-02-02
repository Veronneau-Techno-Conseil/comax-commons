using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Runtime;
using Comax.Commons.Orchestrator.MembershipProvider.Models;

namespace Comax.Commons.Orchestrator.MembershipProvider
{
    public sealed class MembershipTable : Models.IMembershipTable
    {
        private readonly ILogger<MembershipTable> logger;
        private readonly string clusterId;
        private IMembershipStorage membershipStorage;

        public MembershipTable(
            ILogger<MembershipTable> logger,
            IOptions<ClusterOptions> clusterOptions,
            IMembershipStorage membershipStorage)
        {
            this.logger = logger;
            this.clusterId = clusterOptions.Value.ClusterId;
            this.membershipStorage = membershipStorage;
        }

        /// <inheritdoc />
        public Task InitializeMembershipTable(bool tryInitTableVersion)
        {
            return Task.CompletedTask;
        }

        public async Task InitializeMembershipTable(bool tryInitTableVersion, string clusterId)
        {
            await membershipStorage.Init(clusterId);
        }

        /// <inheritdoc />
        public Task DeleteMembershipTableEntries(string deploymentId)
        {
            return DoAndLog(nameof(DeleteMembershipTableEntries), () =>
            {
                return membershipStorage.DeleteMembershipTableEntries(deploymentId);
            });
        }

        /// <inheritdoc />
        public Task<MembershipTableData> ReadRow(SiloAddress key)
        {
            return DoAndLog(nameof(ReadRow), () =>
            {
                return membershipStorage.ReadRow(clusterId, key);
            });
        }

        /// <inheritdoc />
        public Task<MembershipTableData> ReadAll()
        {
            return DoAndLog(nameof(ReadAll), () =>
            {
                return membershipStorage.ReadAll(clusterId);
            });
        }

        /// <inheritdoc />
        public Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion)
        {
            return DoAndLog(nameof(InsertRow), () =>
            {
                return membershipStorage.UpsertRow(clusterId, entry, null, tableVersion);
            });
        }

        /// <inheritdoc />
        public Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion)
        {
            return DoAndLog(nameof(UpdateRow), () =>
            {
                return membershipStorage.UpsertRow(clusterId, entry, etag, tableVersion);
            });
        }

        /// <inheritdoc />
        public Task CleanupDefunctSiloEntries(DateTimeOffset beforeDate)
        {
            return DoAndLog(nameof(CleanupDefunctSiloEntries), () =>
            {
                return membershipStorage.CleanupDefunctSiloEntries(clusterId, beforeDate);
            });
        }

        /// <inheritdoc />
        public Task UpdateIAmAlive(MembershipEntry entry)
        {
            return DoAndLog(nameof(UpdateRow), () =>
            {
                return membershipStorage.UpdateIAmAlive(clusterId,
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
            logger.LogDebug($"{nameof(MembershipTable)}.{actionName} called.");

            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                logger.LogError((int)MongoProviderErrorCode.MembershipTable_Operations, ex, $"{nameof(MembershipTable)}.{actionName} failed. Exception={ex.Message}");

                throw;
            }
        }
    }
}
