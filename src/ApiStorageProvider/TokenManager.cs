using CommunAxiom.Commons.Shared.OIDC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.ApiStorageProvider
{
    public class TokenManager
    {
        private readonly ISettingsProvider _settingsProvider;
        private TokenData _tokenData;
        public TokenManager(ISettingsProvider settingsProvider) 
        {
            _settingsProvider= settingsProvider;
        }

        public DateTime TokenExpires { get; private set; } = DateTime.MinValue;

        public async Task<TokenData> Fetch()
        {
            if (_tokenData != null && DateTime.UtcNow < TokenExpires)
                return _tokenData;

            var settings = await _settingsProvider.GetOIDCSettings();
            TokenClient tokenClient = new TokenClient(settings);
            var (res, data) = await tokenClient.AuthenticateClient(settings.ClientId, settings.Secret, settings.Scopes);
            if (!res)
                throw new UnauthorizedAccessException("Authentication failed");
            _tokenData = data;
            this.TokenExpires = DateTime.UtcNow.AddSeconds(_tokenData.expires_in);
            
            return _tokenData;
        }
    }
}
