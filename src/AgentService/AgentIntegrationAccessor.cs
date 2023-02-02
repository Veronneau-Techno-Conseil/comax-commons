using CommunAxiom.Commons.Client.AgentService.OrchClient;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService
{
    public class AgentIntegrationAccessor : IAgentIntegration
    {
        public Task Clear()
        {
            return AgentIntegration.Clear();
        }

        public IAgentSyncStatus? GetStatus(Guid agentId)
        {
            return AgentIntegration.GetStatus(agentId);
        }

        public async Task Register(Guid id, string token, IConfiguration configuration)
        {
            await AgentIntegration.Register(id, token, configuration);
        }
    }
}
