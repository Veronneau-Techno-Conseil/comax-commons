using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public class AuthRequiredKeepAliveFilter: AuthRequiredAccessControlFilter
    {
        private readonly IGrainFactory _grainFactory;
        public AuthRequiredKeepAliveFilter(IClaimsPrincipalProvider claimsPrincipalProvider, AppIdProvider appIdProvider, IGrainFactory grainFactory): 
            base(claimsPrincipalProvider, appIdProvider)
        {
            _grainFactory = grainFactory;
        }

        protected override Task UserSet()
        {
            var agt = _grainFactory.GetGrain<IAgent>(Guid.Empty);
            _ = agt.IAmAlive();
            return base.UserSet();
        }
    }
}
