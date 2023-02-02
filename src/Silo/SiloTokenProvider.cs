using CommunAxiom.Commons.Shared.OIDC;

using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public class SiloTokenProvider : ITokenProvider
    {
        public SiloTokenProvider() 
        {
            
        }
        public Task<string> FetchToken()
        {
            var t = (string)RequestContext.Get("token");
            if (!string.IsNullOrWhiteSpace(t))
            {
                return Task.FromResult(t);
            }
            return Task.FromResult<string>(null);
        }
    }
}
