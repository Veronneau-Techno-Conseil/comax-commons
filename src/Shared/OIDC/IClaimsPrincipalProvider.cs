
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Shared.OIDC
{
    public interface IClaimsPrincipalProvider
    {
        Task<ClaimsPrincipal?> GetClaimsPrincipal(ITokenProvider tokenProvider);
    }
}
