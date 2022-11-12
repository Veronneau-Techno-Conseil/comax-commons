using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;

namespace Comax.Commons.Orchestrator.ApiMembershipProvider
{
    public class SvcClientFactory : ISvcClientFactory
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly IConfiguration _configuration;

        public SvcClientFactory(ISettingsProvider settingsProvider, IConfiguration configuration)
        {
            _settingsProvider = settingsProvider;
            _configuration = configuration;
        }

        public async Task<ApiRef.RefereeSvc> GetRefereeSvc()
        {
            var settings = await _settingsProvider.GetOIDCSettings();
            TokenClient tokenClient = new TokenClient(settings);
            var (success, res) = await tokenClient.AuthenticateClient(settings.ClientId, settings.Secret, settings.Scopes);
            if (!success) throw new InvalidOperationException("Could not authenticate the client");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", res?.access_token);
            ApiRef.RefereeSvc refereeSvc = new ApiRef.RefereeSvc(_configuration["refereeUrl"], client);
            return refereeSvc;
        }
    }
}
