
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.AgentService.Conf;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public class AgentSyncService : BackgroundService, IMailboxObserver, IAsyncDisposable
    {
        private IAsyncDisposable _subscription;
        private readonly IOrchestratorClientFactory _orchestratorClientFactory;
        private IOrchestratorClient _orchestratorClient;
        private readonly IAgentSyncStatus _connectionStatus;
        private readonly AgentConfig _agentConfig;

        private readonly ILogger<AgentSyncService> _logger;
        public AgentSyncService(ILogger<AgentSyncService> logger, IOrchestratorClientFactory orchestratorClientFactory, IAgentSyncStatus connectionStatus, AgentConfig agentConfig)
            : base()
        {
            _logger = logger;
            _orchestratorClientFactory = orchestratorClientFactory;
            _connectionStatus = connectionStatus;
            _agentConfig = agentConfig;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_connectionStatus.IsConnected)
                    {
                        _connectionStatus.IsConnected = await _orchestratorClientFactory.TestConnection();

                        if (!_connectionStatus.IsConnected)
                        {
                            await Task.Delay(2000);
                            continue;
                        }
                    }

                    if (_orchestratorClient == null || !_orchestratorClient.ClusterClient.IsInitialized)
                    {
                        try { _orchestratorClient?.Dispose(); } catch { }
                        _orchestratorClient = await _orchestratorClientFactory.GetUnmanagedClient();
                        
                    }

                    var mb = await _orchestratorClient.GetEventMailbox();
                    var (coll, handle) = await mb.GetStream();

                    await foreach(var m in coll)
                    {
                        _connectionStatus.MailMessages.Enqueue(m);
                    }
                    await handle.UnsubscribeAsync();
                    
                    while (_connectionStatus.MailMessages.TryDequeue(out var mm))
                    {
                        _connectionStatus.Messages.Enqueue(await mb.GetMessage(mm.MsgId));
                    }
                }
                catch (Exception ex)
                {
                    _connectionStatus.PreviousException = ex;
                    _logger.LogError(ex, "Error running agent sync");
                }
                await Task.Delay(500);
            }
        }

        public Task NewMail(MailMessage message)
        {
            _connectionStatus.IsAuthorized = true;
            _connectionStatus.MailMessages.Enqueue(message);
            _connectionStatus.LastReceived = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _subscription.DisposeAsync();
        }
    }
}
