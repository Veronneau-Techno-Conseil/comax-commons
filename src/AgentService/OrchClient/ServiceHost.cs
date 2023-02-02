using Comax.Commons.Orchestrator.Client;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public class ServiceHost: IHost, IAgentSyncStatus
    {
        public Guid HostId { get; set; }
        public bool IsConnected { get; set; }
        public bool? IsAuthorized { get; set; }
        public Exception PreviousException { get; set; }
        public ConcurrentQueue<Message> Messages { get; set; } = new ConcurrentQueue<Message>();
        public ConcurrentQueue<MailMessage> MailMessages { get; set; } = new ConcurrentQueue<MailMessage>();
        public DateTime LastReceived { get; set; } = DateTime.MinValue;

        public IConfiguration Configuration { get; set; }
        public IServiceProvider Services { get; private set; }

        public string? Token { get; set; }
        public DateTime LastActive { get; set; }

        private List<IAsyncDisposable> _disposables = new List<IAsyncDisposable>();
        private readonly AgentConfig _agentConfig = new AgentConfig();
        public ServiceHost(Guid id, IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Configuration.GetSection("AgentConfig").Bind(_agentConfig);
            ServiceCollection serviceDescriptors = new ServiceCollection();
            
            serviceDescriptors.AddSingleton(Configuration);
            serviceDescriptors.AddLogging(l => l.AddConsole());
            serviceDescriptors.AddSingleton<AgentConfig>(this._agentConfig);
            serviceDescriptors.AddTransient<ITokenProvider, SimpleTokenProvider>(svc => new SimpleTokenProvider(this));
            serviceDescriptors.AddSingleton<ISettingsProvider, ConfigSettingsProvider>(x => new ConfigSettingsProvider("OIDC", configuration));
            serviceDescriptors.AddSingleton<AppIdProvider>();
            serviceDescriptors.AddSingleton<IOrchestratorClientConfig, ClientConfig>();
            serviceDescriptors.AddSingleton<IOutgoingGrainCallFilter, AgentServiceOutgoingFilter>();
            serviceDescriptors.AddSingleton<IOrchestratorClientFactory, Comax.Commons.Orchestrator.Client.ClientFactory>();
            serviceDescriptors.AddSingleton<IAgentSyncStatus>(this);
            serviceDescriptors.AddHostedService<AgentSyncService>(sp =>
            {
                return new AgentSyncService(
                    sp.GetService<ILogger<AgentSyncService>>(),
                    sp.GetService<IOrchestratorClientFactory>(),
                    this,
                    sp.GetService<AgentConfig>()
                    );
            });
            Services = serviceDescriptors.BuildServiceProvider();
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            var logger = Services.GetService<ILogger<AgentSyncService>>();
            try
            {
                var svcs = Services.GetServices<IHostedService>();
                foreach (var s in svcs)
                {
                    _ = s.StartAsync(cancellationToken);
                    if (s is IAsyncDisposable)
                    {
                        _disposables.Add((IAsyncDisposable)s);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ServiceHost startasync failed");
            }
            return Task.CompletedTask;   
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            var svcs = Services.GetServices<IHostedService>();
            foreach (var s in svcs)
            {
                _ = s.StopAsync(cancellationToken);
                if (s is IAsyncDisposable)
                {
                    ((IAsyncDisposable)s).DisposeAsync();
                }
            }
            _disposables.Clear();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            AgentIntegration.Remove(HostId);
        }
    }
}
