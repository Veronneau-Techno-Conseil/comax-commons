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

namespace CommunAxiom.Commons.Client.Silo
{
    class Program
    {
        
        static readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        static void Main(string[] args)
        {
            Console.WriteLine("Starting service...");
            RunMainAsync().GetAwaiter().GetResult();
        }

        private static async Task<int> RunMainAsync()
        {
            Services.Bootstrap();
            await Services.ClusterManagement.SetSilo(Silos.Pilot);

            while (!tokenSource.Token.IsCancellationRequested)
            {
                await Services.ClusterManagement.Heartbeat();
                await Task.Delay(1000);
            }

            return 0;
        }
    }
}
