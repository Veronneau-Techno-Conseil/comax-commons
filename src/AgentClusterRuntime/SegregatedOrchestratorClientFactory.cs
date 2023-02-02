using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.FlowControl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentClusterRuntime
{
    public class SegregatedOrchestratorClientFactory: IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource= new CancellationTokenSource();
        public string? Token { get; set; }
        private readonly SegregatedContext<SegregatedOrchestratorClientFactory> _noFlowContext;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SegregatedOrchestratorClientFactory> _logger;

        public SegregatedOrchestratorClientFactory(IServiceProvider services)
        {
            _orchestratorClientFactory = services.GetService<IOrchestratorClientFactory>();
            _services = services;
            _configuration = _services.GetService<IConfiguration>(); 
            _orchestratorClientFactory = _services.GetService<IOrchestratorClientFactory>();
            _logger = _services.GetService<ILogger<SegregatedOrchestratorClientFactory>>();

            _noFlowContext = new SegregatedContext<SegregatedOrchestratorClientFactory>(()=> Task.FromResult(this), _cancellationTokenSource.Token, _logger);
        }

        public Task<bool> TestConnection()
        {
            return _noFlowContext.Run(x=>x._orchestratorClientFactory.TestConnection());
        }

        public Task WithClusterClient(Func<IOrchestratorClient, Task> action)
        {
            return _noFlowContext.Run(x=>x._orchestratorClientFactory.WithClusterClient(action));
        }

        public Task<TResult> WithClusterClient<TResult>(Func<IOrchestratorClient, Task<TResult>> action)
        {
            return _noFlowContext.Run(x=>x._orchestratorClientFactory.WithClusterClient<TResult>(action));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task SetDefaultToken()
        {
            var tp = _services.GetService<ITokenProvider>();
            this.Token = await tp.FetchToken();
        }
    }
}
