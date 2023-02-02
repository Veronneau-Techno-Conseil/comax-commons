using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Client.Grains.AgentGrain.SyncWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain
{
    public class AgentTokenProvider : ITokenProvider
    {
        public Task<string> FetchToken()
        {
            return Task.FromResult(AuthStateContext.CurrentAuthState.Value.Token);
        }
    }
}
