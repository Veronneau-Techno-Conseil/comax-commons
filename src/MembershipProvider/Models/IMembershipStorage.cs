using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MembershipProvider.Models
{
    public interface IMembershipStorage
    {
        Task CleanupDefunctSiloEntries(string deploymentId, DateTimeOffset beforeDate);

        Task DeleteMembershipTableEntries(string deploymentId);

        Task<IList<Uri>> GetGateways(string deploymentId);

        Task Init(string deploymentId);

        Task<MembershipTableData> ReadAll(string deploymentId);

        Task<MembershipTableData> ReadRow(string deploymentId, SiloAddress address);

        Task UpdateIAmAlive(string deploymentId, SiloAddress address, DateTime iAmAliveTime);

        Task<bool> UpsertRow(string deploymentId, MembershipEntry entry, string etag, TableVersion tableVersion);
    }
}
