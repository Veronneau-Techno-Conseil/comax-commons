using CommunAxiom.Commons.Shared.OIDC;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Orleans.Security
{
    public class AppIdProvider
    {

        private string _accessToken;
        private string _refreshToken;
        private DateTime? _timeout;
        private readonly ISettingsProvider _settingsProvider;
        private ClaimsPrincipal _claimsPrincipal;

        public AppIdProvider(ISettingsProvider settingsProvider) 
        {
            _settingsProvider = settingsProvider;
        }

        public async Task<ClaimsPrincipal> GetClaims()
        {
            await GetAccessToken();
            return _claimsPrincipal;
        }

        public async Task<string> GetAccessToken()
        {
            if(string.IsNullOrWhiteSpace(_accessToken) || DateTime.UtcNow > _timeout) 
            {
                var settings = await _settingsProvider.GetOIDCSettings();
                if (settings == null)
                    return null;
                TokenClient tokenClient = new TokenClient(settings);
                var (success, res) = await tokenClient.AuthenticateClient(settings.ClientId, settings.Secret, settings.Scopes);
                if (success) 
                {
                    _timeout = DateTime.UtcNow.AddSeconds(res.expires_in);
                    _accessToken = res.access_token;
                    _refreshToken = res.refresh_token;
                    var (s, cp) = await tokenClient.RequestIntrospection(settings.ClientId, settings.Secret, _accessToken);
                    if (s)
                        _claimsPrincipal = cp;
                    return _accessToken;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return _accessToken;
            }
        }
    }
}
