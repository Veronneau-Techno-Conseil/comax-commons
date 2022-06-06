using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;

namespace Comax.Commons.Orchestrator.Client
{
    public static class ClientExtensions
    {
        public static void SetupOrleansClient(this IServiceCollection collection)
        {
            collection.AddSingleton<IOrchestratorClientFactory, ClientFactory>();            
        }
        
    }
}