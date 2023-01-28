using CommunAxiom.Commons.Shared.OIDC;
using Orleans;
using Orleans.Runtime;
using O = Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CommunAxiom.Commons.Orleans.Security
{
    public class AccessControlFilter : IIncomingGrainCallFilter, ITokenProvider
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;
        public AccessControlFilter(IClaimsPrincipalProvider claimsPrincipalProvider)
        {
            _claimsPrincipalProvider = claimsPrincipalProvider;
        }

        public Task<string?> FetchToken()
        {
            return Task.FromResult(RequestContext.Get(Config.SECURE_TOKEN_KEY)?.ToString());
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            AccessControlValidationResult? validationResult = null;

            if (!AccessControl.ShouldAuthorize(context))
            {
                await context.Invoke();
                return;
            }

            var cp = await _claimsPrincipalProvider.GetClaimsPrincipal(this);
            ClaimsContainer claimsContainer = new ClaimsContainer();
            claimsContainer.SetPrincipal(cp);

            RequestContext.Set("cp", claimsContainer);
            try
            {
                validationResult = AccessControl.IsAuthorized(context, cp);

                if (validationResult.IsAuthorized)
                {
                    await context.Invoke();
                }
                else
                {
                    throw new AccessControlException()
                    {
                        FailedClaimTypes = validationResult.FailedClaimTypes,
                        IsAuthenticated = validationResult.IsAuthenticated
                    };
                }
            }
            finally
            {
                RequestContext.Remove("cp");
            }
        }
    }
}
