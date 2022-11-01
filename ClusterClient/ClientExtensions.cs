using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using CommunAxiom.Commons.Client.Contracts;
using Orleans.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;

namespace ClusterClient
{
    public static class ClientExtensions
    {
        public static void SetupOrleansClient(this IServiceCollection collection)
        {
            collection.AddSingleton<ICommonsClientFactory, ClientFactory>();            
        }
        
    }
}