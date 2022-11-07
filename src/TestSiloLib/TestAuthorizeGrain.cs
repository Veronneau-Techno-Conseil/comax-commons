using CommunAxiom.Commons.Orleans.Security;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestContracts;

namespace TestSiloLib
{
    public class TestAuthorizeGrain : Grain, ITestAuthorizeGrain
    {
        [AuthorizeClaim]
        public Task<bool> TestAuthenticated()
        {
            return Task.FromResult(true);
        }

        [AuthorizeClaim(ClaimType ="TestClaim", MatchStrategy = AuthorizeClaimAttribute.FilterMode.Contains, ClaimValueFilter = "")]
        public Task<bool> TestClaimContains()
        {
            return Task.FromResult(true);
        }

        [AuthorizeClaim(ClaimType ="TestClaim", MatchStrategy = AuthorizeClaimAttribute.FilterMode.Equals, ClaimValueFilter = "")]
        public Task<bool> TestClaimEquals()
        {
            return Task.FromResult(true);
        }

        [AuthorizeClaim(ClaimType ="TestClaim")]
        public Task<bool> TestClaimExists()
        {
            return Task.FromResult(true);
        }

        [AuthorizeClaim(ClaimType ="TestClaim", MatchStrategy = AuthorizeClaimAttribute.FilterMode.Regex, ClaimValueFilter = "")]
        public Task<bool> TestClaimRegex()
        {
            return Task.FromResult(true);
        }

        [AuthorizeClaim(ClaimType ="TestClaim", MatchStrategy = AuthorizeClaimAttribute.FilterMode.StartsWith, ClaimValueFilter = "")]
        public Task<bool> TestClaimStarts()
        {
            return Task.FromResult(true);
        }

        public Task<bool> TestOpen()
        {
            return Task.FromResult(true);
        }

    }
}
