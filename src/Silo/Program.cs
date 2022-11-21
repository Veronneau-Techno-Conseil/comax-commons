using ClusterClient;
using Comax.Commons.Shared.OIDC;
using CommunAxiom.Commons.Client.Silo.System;
using CommunAxiom.Commons.Client.SiloShared;
using CommunAxiom.Commons.Client.SiloShared.System;
using CommunAxiom.Commons.Orleans.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CommunAxiom.Commons.Client.Silo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting service...");
            var host = Host.CreateDefaultBuilder(args)
                .SetConfiguration(out var cfg)
                .ConfigureServices((host, sc) =>
                {
                    sc.AddLogging(x => x.AddConsole());
                    sc.AddSingleton<ITokenProvider, SiloTokenProvider>();
                    sc.AddSingleton<SecureTokenOutgoingFilter>();

                    sc.SetServerServices();
                    sc.SetupOrleansClient();
                    sc.AddTransient<IClusterManagement, ClusterManagement>();
                }).Build();

            host.Run();
        }
    }
}