using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MembershipProvider.Models
{
    public interface IMembershipTable: Orleans.IMembershipTable
    {
        Task InitializeMembershipTable(bool tryInitTableVersion, string clusterId);

    }
}
