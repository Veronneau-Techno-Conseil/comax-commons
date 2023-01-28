using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentClusterRuntime
{
    public static class ServiceProviderExtensions
    {
        public static SegregatedOrchestratorClientFactory SegregatedOrchClientFactory(this IServiceProvider serviceProvider)
        {
            return new SegregatedOrchestratorClientFactory(serviceProvider);
        }

        public static async Task<string> GetComaxUri(this IServiceProvider serviceProvider, string suffix)
        {
            var sp = serviceProvider.GetService<ISettingsProvider>();
            var settings = await sp.GetOIDCSettings();
            return $"com://{settings.ClientId}/{suffix}".ToLower();
        }


        public static async Task<string> GetAgentUri(this IServiceProvider serviceProvider)
        {
            var sp = serviceProvider.GetService<ISettingsProvider>();
            var settings = await sp.GetOIDCSettings();
            return $"com://{settings.ClientId}";
        }
    }
}