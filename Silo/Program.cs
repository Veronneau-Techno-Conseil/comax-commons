using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using Orleans.Security.Clustering;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using CommunAxiom.Commons.Client.Grains.AccountGrain;
using DatasourceGrain;
using DataTransferGrain;
using IngestionGrain;
using PortfolioGrain;
using ProjectGrain;
using ReplicationGrain;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Replication;
using Comax.Commons.StorageProvider.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Orleans.Runtime;
using Orleans.Providers;
using CommunAxiom.Commons.Client.Silo.System;
using ClusterClient;
using Microsoft.Extensions.Hosting;
using CommunAxiom.Commons.Client.SiloShared;
using CommunAxiom.Commons.Client.SiloShared.System;

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
                    sc.SetServerServices();
                    sc.SetupOrleansClient();
                    sc.AddTransient<IClusterManagement, ClusterManagement>();
                    sc.AddLogging(lb => lb.AddConsole());
                }).Build();

            host.Run();
        }

    }
}
