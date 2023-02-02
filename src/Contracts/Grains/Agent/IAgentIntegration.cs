using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public interface IAgentIntegration
    {
        Task Register(Guid id, string token, IConfiguration configuration);
        IAgentSyncStatus? GetStatus(Guid agentId);
        Task Clear();
    }
}
