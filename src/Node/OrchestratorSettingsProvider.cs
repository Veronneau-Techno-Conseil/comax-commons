using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.DotnetSdk.Helpers.OIDC;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator
{
    public class OrchestratorSettingsProvider : ISettingsProvider
    {
        private readonly IConfiguration _configuration;

        public OrchestratorSettingsProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<OIDCSettings> GetOIDCSettings()
        {
            OIDCSettings settings = new OIDCSettings();
            _configuration.Bind("OIDC", settings);
            return Task.FromResult(settings);
        }
    }
}
