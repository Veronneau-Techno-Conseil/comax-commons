using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using CommunAxiom.Commons.Client.Contracts;
using Orleans.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;

namespace CommunAxiom.Commons.Client.ClusterClient
{
    public static class ClientExtensions
    {
        public static void SetupOrleansClient(this IServiceCollection collection, string configFile)
        {
            collection.AddSingleton<ICommonsClientFactory>(sp => 
            {
                var fact = new ClientFactory(sp, configFile);
                return fact;
            });
        }
        
    }
}