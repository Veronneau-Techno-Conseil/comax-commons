using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared.RuleEngine;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.FlowControl;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Orleans.Streams;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain.SyncWorker
{
    public class AgentSyncState
    {
        private readonly SegregatedContext<AgentSyncState> _noFlowContext;
        private Guid _syncId;

        internal string Token { get; private set; }
        public bool? IsAuthorized { get; private set; }
        public ConcurrentQueue<Message> Messages { get; private set; } = new ConcurrentQueue<Message>();


        private IOrchestratorClient _orchestratorClient;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;
        private readonly IServiceProvider _services;
        public bool IsConnected { get; private set; }
        private readonly IConfiguration _configuration;
        private readonly IAgentIntegration _agentIntegration;
        private readonly string _uri;
        public AgentSyncState(Guid syncId, string uri, string token, IConfiguration configuration, IOrchestratorClientFactory orchestratorClientFactory,
            CancellationToken cancellationToken, IServiceProvider services, IAgentIntegration agentIntegration)
        {
            Token = token;
            Messages = new ConcurrentQueue<Message>();
            _uri= uri;
            _syncId = syncId;
            _orchestratorClientFactory = orchestratorClientFactory;
            _services = services;
            _noFlowContext = new SegregatedContext<AgentSyncState>(Setup, cancellationToken, services.GetService<ILogger<SegregatedContext<AgentSyncState>>>());
            _configuration = configuration;
            _agentIntegration = agentIntegration;
        }

        public async Task<AgentSyncState> Setup()
        {
            await GetOrchClient();
            return this;
        }

        private async Task<IOrchestratorClient> GetOrchClient()
        {
            if (_orchestratorClient == null)
            {
                _orchestratorClient = await _orchestratorClientFactory.GetUnmanagedClient();
                IsConnected = true;
            }
            return _orchestratorClient;
        }


        public async Task Refresh(string token)
        {
            Token = token;
            await _agentIntegration.Register(_syncId, token, _configuration);
        }

        public async Task<List<Message>> IAmAlive()
        {
            try
            {
                return await _noFlowContext.Run(async (state) =>
                {
                    AuthStateContext.State.Value = new AuthStateContext.AuthState() { Token = Token };
                    if (!IsAuthorized.HasValue || IsAuthorized.Value)
                    {
                        var actor = await _orchestratorClient.GetActor();
                        await actor.IAmAlive();
                        IsAuthorized = true;
                    }

                    await _agentIntegration.Register(_syncId, Token, _configuration);
                    var status = _agentIntegration.GetStatus(_syncId);

                    List<Message> toSend = new List<Message>();

                    if (status == null)
                        return toSend;

                    if (status.Messages != null)
                    {
                        while (status.Messages.TryDequeue(out var message))
                        {
                            toSend.Add(message);
                        }
                    }
                    return toSend;
                });            
            }
            catch (AccessControlException ex)
            {
                IsAuthorized = false;
            }
            catch(Exception ex) 
            {
                throw;
            }
            finally
            {
                AuthStateContext.State.Value = AuthStateContext.AuthState.Default;
            }
            return null;
        }

    }

    public enum SyncStatus
    {
        Connected,
        Disconnected,
        Failed
    }
}
