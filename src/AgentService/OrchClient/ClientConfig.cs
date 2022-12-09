using Comax.Commons.Orchestrator.ApiMembershipProvider;
using Comax.Commons.Orchestrator.Client;
using Comax.Commons.Shared.OIDC;
using CommunAxiom.Commons.Client.Silo;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public class ClientConfig : IOrchestratorClientConfig
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IConfiguration _configuration;
        public ClientConfig(IServiceProvider serviceProvider, ISettingsProvider settingsProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _settingsProvider = settingsProvider;
            _configuration = configuration;
        }
        public void Configure(IServiceCollection sc)
        {
            sc.AddSingleton(_configuration);
            sc.AddLogging(l => l.AddConsole());
            sc.AddTransient<IOutgoingGrainCallFilter, CommonsAgentOutgoingFilter>();
            sc.AddSingleton<ITokenProvider>(new TokenWrapper(_serviceProvider.GetService<ITokenProvider>().FetchToken().GetAwaiter().GetResult()));
            sc.AddSingleton(_serviceProvider.GetService<AppIdProvider>());
            
            sc.AddSingleton(_settingsProvider);
            sc.AddSingleton<ISvcClientFactory, SvcClientFactory>();
            sc.AddApiProvider(c => _configuration.GetSection("membership").Bind(c));
        }

        public class TokenWrapper : ITokenProvider
        {
            private readonly string _token;
            public TokenWrapper(string token)
            {
                _token= token;
            }
            public Task<string> FetchToken()
            {
                return Task.FromResult(_token);
            }
        }
    }
}
