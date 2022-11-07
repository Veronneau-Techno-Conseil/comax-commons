using Comax.Commons.Shared.OIDC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public class SiloTokenProvider : ITokenProvider
    {
        public Task<string> FetchToken()
        {
            return Task.FromResult<string>(null);
        }
    }
}
