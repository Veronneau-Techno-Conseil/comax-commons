using Comax.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public class ClaimsProvider : IClaimsPrincipalProvider
    {
        private readonly ISettingsProvider _settingsProvider;

        public ClaimsProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipal(ITokenProvider tokenProvider)
        {
            var token = await tokenProvider.FetchToken();

            if (string.IsNullOrEmpty(token))
                return null;

            var settings = await _settingsProvider.GetOIDCSettings();

            var client = new TokenClient(settings);
            
            var (completed, result) = await client.RequestIntrospection(settings.ClientId, settings.Secret, token);
            if (completed)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
