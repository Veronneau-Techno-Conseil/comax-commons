using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using Microsoft.Extensions.Configuration;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain.ConnectionMonitor
{
    public class ConnectionMonitorWorker : Grain, IConnectionMonitor
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CWBusiness _cWBusiness;
        private IDisposable _connectionMonitor;
        private readonly AgentConfig _agentConfig;

        public ConnectionMonitorWorker(IOrchestratorClientFactory orchestratorClientFactory, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _cWBusiness = new CWBusiness(orchestratorClientFactory, _cancellationTokenSource.Token, serviceProvider);
            _agentConfig = new AgentConfig();
            configuration.GetSection("AgentConfig").Bind(_agentConfig);
        }

        public override Task OnActivateAsync()
        {
            _connectionMonitor = RegisterTimer(x => this.RunCheck(), true,
                TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(_agentConfig.ConnectionCheckPeriod));

            return base.OnActivateAsync();
        }

        private async Task RunCheck()
        {
            await _cWBusiness.CheckConnection();
            DelayDeactivation(TimeSpan.FromSeconds(_agentConfig.ConnectionCheckPeriod));
        }

        public override Task OnDeactivateAsync()
        {
            _cancellationTokenSource.Cancel();
            _connectionMonitor.Dispose();
            _cWBusiness.Dispose();
            return base.OnDeactivateAsync();
        }

        public Task EnsureStarted()
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsConnected()
        {
            return _cWBusiness.IsConnected();
        }
    }
}
