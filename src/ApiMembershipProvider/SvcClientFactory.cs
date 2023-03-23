using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.DotnetSdk.Helpers.OIDC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Comax.Commons.CommonsShared.ApiMembershipProvider
{
    public class SvcClientFactory : ISvcClientFactory
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ApiMembershipConfig _configuration;

        public SvcClientFactory(IOptions<ApiMembershipConfig> apiMembershipConfig, ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
            _configuration = apiMembershipConfig.Value;
        }

        private TokenData tokenData = null;
        private DateTime? expires = null;

        public async Task<ApiRef.RefereeSvc> GetRefereeSvc()
        {
            if (tokenData == null || expires != null && (expires.Value - DateTime.UtcNow) < TimeSpan.FromMinutes(4)) { 
                var settings = await _settingsProvider.GetOIDCSettings();
                TokenClient tokenClient = new TokenClient(settings);
                
                var (success, res) = await tokenClient.AuthenticateClient(settings.Scopes);
                if (!success) throw new InvalidOperationException("Could not authenticate the client");
                tokenData = res;
                                
                expires = DateTime.UtcNow + TimeSpan.FromSeconds(tokenData.expires_in);
            }
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenData?.access_token);
            ApiRef.RefereeSvc refereeSvc = new ApiRef.RefereeSvc(_configuration.Host, client);
            return refereeSvc;
        }
    }
}
