using CommunAxiom.Commons.Orleans.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests
{
    public static class PrincipalExtensions
    {
        public static string? GetUri(this ClaimsPrincipal cp)
        {
            return cp?.FindFirst(Constants.URI_CLAIM).Value;
        }

        public static string? GetOwner(this ClaimsPrincipal cp)
        {
            return cp?.FindFirst(Constants.OWNER_CLAIM).Value;
        }
    }
}
