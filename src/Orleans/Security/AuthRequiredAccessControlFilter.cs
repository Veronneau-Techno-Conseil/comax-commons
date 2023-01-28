
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
    public class AuthRequiredAccessControlFilter : IIncomingGrainCallFilter, ITokenProvider
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;
        private readonly AppIdProvider _appIdProvider;
        public AuthRequiredAccessControlFilter(IClaimsPrincipalProvider claimsPrincipalProvider, AppIdProvider appIdProvider)
        {
            _claimsPrincipalProvider = claimsPrincipalProvider;
            _appIdProvider = appIdProvider;
        }

        public async Task<string?> FetchToken()
        {
            var token = (RequestContext.Get(Config.SECURE_TOKEN_KEY))?.ToString();

            if (string.IsNullOrWhiteSpace(token) && RequestContext.Get("__si") != null)
            {
                token = await _appIdProvider.GetAccessToken();
            }

            return token;
        }

        public async Task Invoke(IIncomingGrainCallContext context)
        {
            AccessControlValidationResult? validationResult = null;

            if(AccessControl.PassthroughAuthorize(context))
            {
                await context.Invoke();
                return;
            }

            var cp = await _claimsPrincipalProvider.GetClaimsPrincipal(this);

            if(cp == null && RequestContext.Get("__si") == null)
            {
                throw new AccessControlException()
                {
                    FailedClaimTypes = null,
                    IsAuthenticated = false
                };
            }

            ClaimsContainer claimsContainer = new ClaimsContainer();
            claimsContainer.SetPrincipal(cp);

            RequestContext.Set("cp", claimsContainer);

            if (!AccessControl.ShouldAuthorize(context))
            {
                await context.Invoke();
                return;
            }

            if(RequestContext.Get("__si") == null)
                await this.UserSet();
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

        protected virtual Task UserSet()
        {
            return Task.CompletedTask;
        }
    }
}
