using Comax.Commons.Shared.OIDC;
using System.Security.Claims;

namespace CommunAxiom.Commons.Shared.OIDC
{
    public class OIDCClaimsProvider : IClaimsPrincipalProvider
    {
        private readonly ISettingsProvider _settingsProvider;

        public OIDCClaimsProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public async Task<ClaimsPrincipal?> GetClaimsPrincipal(ITokenProvider tokenProvider)
        {
            var token =  await tokenProvider.FetchToken();

            if(string.IsNullOrEmpty(token))
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
