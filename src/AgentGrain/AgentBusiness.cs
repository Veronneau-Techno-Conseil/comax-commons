using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AgentGrain
{
    public class AgentBusiness : IUserContextAccessor, IMailboxObserver, IDisposable
    {
        private readonly AgentRepo _agentRepo;
        private readonly IComaxGrainFactory _comaxGrainFactory;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;
        private IOrchestratorClient _orchestratorClient;
        private Func<IAsyncStream<Message>> _getMessageStream;

        public AgentBusiness(AgentRepo agentRepo, IComaxGrainFactory comaxGrainFactory, IOrchestratorClientFactory orchestratorClientFactory, Func<IAsyncStream<Message>> messageStreamFactory)
        {
            _agentRepo = agentRepo;
            _comaxGrainFactory = comaxGrainFactory;
            _orchestratorClientFactory = orchestratorClientFactory;
            _getMessageStream = messageStreamFactory;
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
            var cl = await GetOrchClient();
            var mb = await cl.GetEventMailbox();
            await mb.Subscribe(this);

            var strm = _getMessageStream();
            var authState = await this._agentRepo.GetUserAuthState(this.GetUser().GetUri());
            foreach(var item in authState.MailMessages)
            {
                var message = await mb.GetMessage(item.MsgId);
                await strm.OnNextAsync(message);
            }
        }

        public async Task CheckConnection()
        {
            var isConnected = await _orchestratorClientFactory.TestConnection();
            await this._agentRepo.ConnectionState(isConnected);
        }

        public async Task NewMail(MailMessage message)
        {
            await _agentRepo.AddUserMessage(message);
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
            throw new NotImplementedException();
        }

        
    }
}
