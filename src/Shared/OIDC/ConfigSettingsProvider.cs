using CommunAxiom.DotnetSdk.Helpers.OIDC;
using Microsoft.Extensions.Configuration;
using Orleans.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared.OIDC
{
    public class ConfigSettingsProvider : ISettingsProvider
    {
        private readonly IConfiguration configuration;
        private readonly string section;
        public ConfigSettingsProvider(string section, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.section = section;
        }
        public Task<OIDCSettings> GetOIDCSettings()
        {
            OIDCSettings settings = new OIDCSettings();
            this.configuration.GetSection(this.section).Bind(settings);
            return Task.FromResult(settings);
        }
    }
}
