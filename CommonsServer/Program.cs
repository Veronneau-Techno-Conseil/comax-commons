using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using Orleans.Security.Clustering;
using Microsoft.Extensions.DependencyInjection;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using CommunAxiom.Commons.Client.Grains.AccountGrain;
using System.Threading;
using PortfolioGrain;
using ReplicationGrain;
using ProjectGrain;
using CommunAxiom.Commons.Client.Contracts.Replication;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using DatasourceGrain;
using DataTransferGrain;
using IngestionGrain;
using Microsoft.Extensions.Configuration;
using Orleans.Security;
using CommunAxiom.Commons.Client.Contracts.Configuration;

namespace CommunAxiom.Commons.Client.Silo
{
    class Program
    {

        
        static void Main(string[] args)
        {
            Console.WriteLine("Orleans Silo is Launching");
            RunMainAsync().GetAwaiter().GetResult();
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                await PilotSilo.StartSilo();
                
                
                Console.WriteLine("\n\n The Silo is Up\n\n");
                Console.ReadLine();
                await PilotSilo.StopSilo();

                // Wait for the silo to completely shutdown before exiting. 
                

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

    }
}
