using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Orleans.Security;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator
{
    public class ClientTokenProvider : ITokenProvider
    {
        const string IMPERSONATED_TOKEN_KEY = "IMP_TOKEN_KEY";

        private readonly ISettingsProvider _settingsProvider;

        public ClientTokenProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        public async Task<string> FetchToken()
        {
            //TODO: Manage expires in
            var impersonated = RequestContext.Get(IMPERSONATED_TOKEN_KEY);
            
            if (impersonated == null)
            {
                var settings = await _settingsProvider.GetOIDCSettings();
                var secureToken = RequestContext.Get(Config.SECURE_TOKEN_KEY)?.ToString();
                if (!string.IsNullOrWhiteSpace(secureToken))
                {
                    TokenClient tokenClient = new TokenClient(settings);
                    var (res,data) = await tokenClient.Impersonate(secureToken);
                    if (res)
                        RequestContext.Set(IMPERSONATED_TOKEN_KEY, data.access_token);
                    else
                        throw new Exception("Impersonation failed");
                }
                else
                    throw new Exception("No secure token available");
            }

            impersonated = RequestContext.Get(IMPERSONATED_TOKEN_KEY);
            return impersonated.ToString();
        }
    }
}
