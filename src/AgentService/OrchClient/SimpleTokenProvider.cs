using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public class SimpleTokenProvider : ITokenProvider
    {
        private readonly IAgentSyncStatus _status;

        public SimpleTokenProvider(IAgentSyncStatus status)
        {
            _status= status;
        }

        public Task<string?> FetchToken()
        {
            return Task.FromResult(_status.Token);   
        }
    }
}
