using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain
{
    [AuthorizeClaim]
    [Reentrant]
    public class Agent : Grain, IAgent, IDisposable
    {
        private AgentBusiness _agentBusiness;
        private readonly AgentRepo _repo;
        private IDisposable _iAmAliveTicker, _connectionMonitor, _stateMonitor;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;
        private readonly ITokenProvider _tokenProvider;
        private readonly AppIdProvider _appIdProvider;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly AgentConfig _agentConfig;

        public Agent([PersistentState("agentState")] IPersistentState<AgentState> agentState, IOrchestratorClientFactory orchestratorClientFactory, ITokenProvider tokenProvider, AppIdProvider appIdProvider, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _repo = new AgentRepo(agentState);
            _tokenProvider = tokenProvider;
            _orchestratorClientFactory = orchestratorClientFactory;
            _appIdProvider = appIdProvider;
            _cancellationTokenSource = new CancellationTokenSource();
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILogger<Agent>>();
            _agentConfig = new AgentConfig();
            configuration.GetSection("AgentConfig").Bind(_agentConfig);
        }

        

        public override Task OnActivateAsync()
        {
            var gf = new GrainFactory(this.GrainFactory, this.GetStreamProvider);

            _agentBusiness = new AgentBusiness(_repo, 
                                               gf, 
                                               _tokenProvider,
                                               _appIdProvider, 
                                               _cancellationTokenSource.Token,
                                               _serviceProvider,
                                               _logger,
                                               _agentConfig);

            _iAmAliveTicker = RegisterTimer(
                x => {
                    return this.IAmAlive(); 
                }, true,
                TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(_agentConfig.LiveTickerPeriod));

            _stateMonitor = RegisterTimer(x => _agentBusiness.EnsureSaved(), true,
                TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(_agentConfig.SaveStatePeriod));
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            return base.OnDeactivateAsync();
        }

        public Task<UserAuthState> GetCommonsAuthState()
        {
            return _agentBusiness.GetCommonsAuthState();
        }

        public Task<UserAuthState> GetCurrentUserAuthState()
        {
            return _agentBusiness.GetCurrentUserAuthState();
        }


        [AuthorizePassthrough]
        public Task EnsureStarted()
        {
            return Task.CompletedTask;
        }

        public Task IAmAlive()
        {
            this.DelayDeactivation(TimeSpan.FromSeconds(_agentConfig.LiveTickerPeriod));
            return _agentBusiness.IAmAlive();
        }

        public void Dispose()
        {
            try
            {
                _iAmAliveTicker.Dispose();
                _connectionMonitor.Dispose();
                _stateMonitor.Dispose();
            }
            catch { }
        }
    }
}
