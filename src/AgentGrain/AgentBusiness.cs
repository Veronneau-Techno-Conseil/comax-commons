using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using Comax.Commons.Shared.OIDC;
using CommunAxiom.Commons.Client.AgentService.OrchClient;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain
{
    public class AgentBusiness : IUserContextAccessor, IDisposable
    {
        private readonly AgentRepo _agentRepo;
        private readonly IComaxGrainFactory _comaxGrainFactory;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;
        private IOrchestratorClient _orchestratorClient;
        private Func<IAsyncStream<Message>> _getMessageStream;
        private readonly ITokenProvider _tokenProvider;
        private readonly AppIdProvider _appIdProvider;

        public AgentBusiness(AgentRepo agentRepo, IComaxGrainFactory comaxGrainFactory, IOrchestratorClientFactory orchestratorClientFactory, Func<IAsyncStream<Message>> messageStreamFactory, ITokenProvider tokenProvider, AppIdProvider appIdProvider)
        {
            _agentRepo = agentRepo;
            _comaxGrainFactory = comaxGrainFactory;
            _orchestratorClientFactory = orchestratorClientFactory;
            _getMessageStream = messageStreamFactory;
            _tokenProvider = tokenProvider;
            _appIdProvider = appIdProvider;
        }


        private async Task<IOrchestratorClient> GetOrchClient()
        {
            if(_orchestratorClient == null)
            {
                _orchestratorClient = await _orchestratorClientFactory.GetUnmanagedClient();
            }
            return _orchestratorClient;
        }

        public async Task IAmAlive()
        {
            bool isCluster = false;
            var token = await _tokenProvider.FetchToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                token = await _appIdProvider.GetAccessToken();
                isCluster = true;
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }
            
            try
            {
                string uri = null;
                if (isCluster)
                {
                    uri = (await _appIdProvider.GetClaims()).GetUri();
                }
                else
                {
                    uri = this.GetUser().GetUri();
                }
                var authstate = await this._agentRepo.GetUserAuthState(uri);
                if(!authstate.IsAuthorised || authstate.Subscription == Guid.Empty)
                {
                    var id = AgentIntegration.Register(token, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10));
                    authstate.Subscription = id;
                    await _agentRepo.SetUserAuthState(authstate);
                }

                var auth = AgentIntegration.IsAuthorized(authstate.Subscription);
                if (auth.HasValue)
                {
                    authstate.IsAuthorised = auth.Value;
                }

                await _agentRepo.SetUserAuthState(authstate);

                var strm = _getMessageStream();
                if(strm != null)
                {
                    var messages = AgentIntegration.Read(authstate.Subscription);
                    foreach (var item in messages)
                    {
                        await strm.OnNextAsync(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task CheckConnection()
        {
            var isConnected = AgentIntegration.IsConnected;
            await this._agentRepo.ConnectionState(isConnected);
        }

        public async Task<UserAuthState> GetCommonsAuthState()
        {
            var act = this._comaxGrainFactory.GetGrain<IAccount>(Guid.Empty);
            var dt = await act.GetDetails();
            return await _agentRepo.GetUserAuthState(dt.ApplicationUri);
        }

        public Task<UserAuthState> GetCurrentUserAuthState()
        {
            return _agentRepo.GetUserAuthState(this.GetUser().GetUri());
        }

        public Task<OrchestratorConnectionState> GetOrchestratorConnectionState()
        {
            return _agentRepo.GetConnectionState();
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
