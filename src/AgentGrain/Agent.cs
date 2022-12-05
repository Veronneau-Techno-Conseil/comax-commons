using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.ClusterEventStream.Extentions;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace AgentGrain
{
    [AuthorizeClaim]
    [Reentrant]
    public class Agent : Grain, IAgent, IDisposable
    {
        private AgentBusiness _agentBusiness;
        private readonly AgentRepo _repo;
        private IDisposable _iAmAliveTicker, _connectionMonitor;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;

        public Agent(IPersistentState<AgentState> agentState, IOrchestratorClientFactory orchestratorClientFactory)
        {
            _repo = new AgentRepo(agentState);
        }
        
        public override Task OnActivateAsync()
        {
            var gf = new GrainFactory(this.GrainFactory);

            var fn = () =>
            {
                var streamProvider = GetStreamProvider(CommunAxiom.Commons.Orleans.Constants.ImplicitStream);
                return streamProvider.GetEventStream();
            };

            _agentBusiness = new AgentBusiness(_repo, gf, _orchestratorClientFactory, fn);
            _iAmAliveTicker = RegisterTimer(x => _agentBusiness.IAmAlive(), true,
                TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5)); 
            _connectionMonitor = RegisterTimer(x => _agentBusiness.CheckConnection(), true,
                TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30)); 
            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
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

        public Task<OrchestratorConnectionState> GetOrchestratorConnectionState()
        {
            return _agentBusiness.GetOrchestratorConnectionState();
        }

        public Task IAmAlive()
        {
            return _agentBusiness.IAmAlive();
        }


        public void Dispose()
        {
            try
            {
                _iAmAliveTicker.Dispose();
                _connectionMonitor.Dispose();
            }
            catch { }
        }
    }
}
