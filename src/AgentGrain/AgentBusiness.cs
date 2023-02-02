using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.Client.Grains.AgentGrain.SyncWorker;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain
{
    public class AgentBusiness : IUserContextAccessor, IDisposable
    {
        private readonly AgentRepo _agentRepo;
        private readonly IComaxGrainFactory _comaxGrainFactory;
        
        private IOrchestratorClient _orchestratorClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly AppIdProvider _appIdProvider;
        
        private readonly ILogger _logger;
        
        public AgentBusiness(AgentRepo agentRepo, IComaxGrainFactory comaxGrainFactory,
            ITokenProvider tokenProvider, AppIdProvider appIdProvider, 
            CancellationToken cancellationToken, IServiceProvider serviceProvider, ILogger logger, AgentConfig agentConfig)
        {
            _agentRepo = agentRepo;
            _comaxGrainFactory = comaxGrainFactory;
            _tokenProvider = tokenProvider;
            _appIdProvider = appIdProvider;
            _logger = logger;
        }


        public async Task IAmAlive()
        {
            var cm = _comaxGrainFactory.GetGrain<IConnectionMonitor>(Guid.Empty);
            var isConnected = await cm.IsConnected();

            if (!isConnected)
                return;

            var uc = await  new UserOrDefault(_tokenProvider, _appIdProvider).Init();
            
            if (string.IsNullOrWhiteSpace(uc.Token))
            {
                return;
            }

            try
            {
                var reg = _comaxGrainFactory.GetGrain<IUriRegistry>(uc.Uri);
                var authstate = await this._agentRepo.GetUserAuthState(uc.Uri);
                authstate ??= new UserAuthState() { PrincipalId = uc.Uri };
                var id = await reg.GetOrCreate();    
                authstate.Subscription = id;
                authstate.LastDate = DateTime.UtcNow;
                await _agentRepo.SetUserAuthState(authstate);

                var grain = _comaxGrainFactory.GetGrain<IAgtSyncWorker>(authstate.Subscription);
                _ = grain.IAmAlive(uc.Token, authstate.Subscription, uc.Uri);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed calling I Am Alive");
            }
        }

        
        public async Task<UserAuthState> GetCommonsAuthState()
        {
            var act = this._comaxGrainFactory.GetGrain<IAccount>(Guid.Empty);
            var dt = await act.GetDetails();
            var ret = await _agentRepo.GetUserAuthState(dt.ApplicationUri);
            if(ret.Subscription != Guid.Empty)
            {
                var worker = this._comaxGrainFactory.GetGrain<IAgtSyncWorker>(ret.Subscription);
                ret.IsAuthorised = await worker.IsAuthorized();
            }
            return ret;
        }

        public async Task<UserAuthState> GetCurrentUserAuthState()
        {
            var ret = await _agentRepo.GetUserAuthState(this.GetUser().GetUri());
            if (ret.Subscription != Guid.Empty)
            {
                var worker = this._comaxGrainFactory.GetGrain<IAgtSyncWorker>(ret.Subscription);
                ret.IsAuthorised = await worker.IsAuthorized();
            }
            return ret;
        }

        public void Dispose()
        {
            _orchestratorClient?.Dispose();
        }

        internal Task EnsureSaved()
        {
            return _agentRepo.EnsureSaved();
        }
    }
}
