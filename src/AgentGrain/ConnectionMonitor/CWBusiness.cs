using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Grains.AgentGrain.SyncWorker;
using CommunAxiom.Commons.Shared.FlowControl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain.ConnectionMonitor
{
    public class CWBusiness : IDisposable
    {
        private bool _isDisposed = false;
        private bool _isConnected = false;
        private readonly SegregatedContext<CWBusiness> _segregatedContext;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;

        public CWBusiness(IOrchestratorClientFactory orchestratorClientFactory, CancellationToken cancellationToken, IServiceProvider serviceProvider)
        {
            _orchestratorClientFactory = orchestratorClientFactory;
            _segregatedContext = new SegregatedContext<CWBusiness>(() => Task.FromResult(this), cancellationToken, 
                serviceProvider.GetService<ILogger<SegregatedContext<CWBusiness>>>());
        }

        public async Task CheckConnection()
        {
            if (_isDisposed)
                return;
            await _segregatedContext.WaitForContext();
            await _segregatedContext.Run(async c =>
            {
                c._isConnected = await _orchestratorClientFactory.TestConnection();
            });
        }

        public async Task<bool> IsConnected()
        {
            return this._isConnected;
        }

        public void Dispose()
        {
            _segregatedContext.Dispose();
            _isDisposed = true;
        }
    }
}
