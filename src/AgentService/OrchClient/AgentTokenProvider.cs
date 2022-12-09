using Comax.Commons.Shared.OIDC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public class AgentTokenProvider : ITokenProvider
    {
        public Task<string> FetchToken()
        {
            return Task.FromResult(AuthStateContext.CurrentAuthState.Value.Token);
        }
    }
}
