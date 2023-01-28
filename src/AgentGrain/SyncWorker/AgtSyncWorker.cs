using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.AgentClusterRuntime;
using CommunAxiom.Commons.Client.AgentClusterRuntime.Extentions;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain.SyncWorker
{
    public class AgtSyncWorker : Grain, IAgtSyncWorker, IUserContextAccessor
    {
        private readonly IConfiguration _configuration;
        private readonly AgentConfig _agentConfig;
        private AgentSyncState _agentSyncState;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly IServiceProvider _serviceProvider;
        private readonly IAgentIntegration _agentIntegration;
        private readonly ILogger<AgtSyncWorker> _logger;
        private readonly ISettingsProvider _settingsProvider;

        public AgtSyncWorker(IConfiguration configuration, IOrchestratorClientFactory orchestratorClientFactory, 
            IServiceProvider serviceProvider, IAgentIntegration agentIntegration, ILogger<AgtSyncWorker> logger, ISettingsProvider settingsProvider)
        {
            _configuration = configuration;
            _agentConfig = new AgentConfig();
            _configuration.GetSection("AgentConfig").Bind(_agentConfig);
            _orchestratorClientFactory = orchestratorClientFactory;
            _serviceProvider = serviceProvider;
            _agentIntegration = agentIntegration;
            _logger= logger;
            _settingsProvider = settingsProvider;
        }

        public async Task IAmAlive(string token, Guid id, string uri)
        {
            if (_agentSyncState == null)
            {
                
                _agentSyncState = new AgentSyncState(id, uri, token, _configuration,  _orchestratorClientFactory,
                                               _cancellationTokenSource.Token, _serviceProvider, _agentIntegration);
            }
            _agentSyncState.Refresh(token);
            var msgs =  await _agentSyncState.IAmAlive();

            var streamProvider = GetStreamProvider(OrleansConstants.Streams.ImplicitStream);
            var key = this.GetPrimaryKey();
            var stream = streamProvider.GetStream<Message>(
                    key, OrleansConstants.StreamNamespaces.DefaultNamespace);
            foreach (var msg in msgs)
                await stream.OnNextAsync(msg);
        }


        public Task<bool?> IsAuthorized()
        {
            return Task.FromResult(_agentSyncState.IsAuthorized);
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }

        public override Task OnDeactivateAsync()
        {
            _cancellationTokenSource.Cancel();
            return base.OnDeactivateAsync();
        }
    }
}
