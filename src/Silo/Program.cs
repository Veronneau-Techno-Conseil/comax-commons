using CommunAxiom.Commons.Client.ClusterClient;
using Comax.Commons.Orchestrator.Client;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.AgentService.Conf;
using CommunAxiom.Commons.Client.AgentService.OrchClient;
using CommunAxiom.Commons.Client.Silo.System;
using CommunAxiom.Commons.Client.SiloShared;
using CommunAxiom.Commons.Client.SiloShared.System;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using CommunAxiom.Commons.Client.Grains.AgentGrain;
using System.Threading;

namespace CommunAxiom.Commons.Client.Silo
{
    class Program
    {
        static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.WriteLine("Starting service...");
            var host = Host.CreateDefaultBuilder(args)
                .SetConfiguration(out var cfg, "./config.json")
                .ConfigureLogging(l=>l.AddConsole())
                .ConfigureServices((host, sc) =>
                {
                    
                    sc.AddLogging(x => x.AddConsole());
                    sc.AddSingleton<ITokenProvider, AgentTokenProvider>();
                    sc.AddSingleton<IOutgoingGrainCallFilter,SecureTokenOutgoingFilter>();
                    sc.AddSingleton<AppIdProvider>();
                    sc.AddSingleton<IOrchestratorClientConfig, ClientConfig>();
                    sc.AddSingleton<ISettingsProvider, StaticSettingsProvider>();
                    sc.AddSingleton<IOrchestratorClientFactory, Comax.Commons.Orchestrator.Client.ClientFactory>();
                    sc.AddSingleton<IClusterManagement, ClusterManagement>();
                    sc.SetServerServices();
                    sc.SetupOrleansClient("./config.json");
                }).Build();

            AgentIntegration.CancellationToken = cancellationTokenSource.Token;
            host.WaitForShutdownAsync().ContinueWith(async x => {
                cancellationTokenSource.Cancel();
                await AgentIntegration.Clear(); 
            });

            host.Run();
        }
    }
}
