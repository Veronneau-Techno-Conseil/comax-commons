using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Messaging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.CommonsShared.ApiMembershipProvider
{
    public static class MongoDBClientExtensions
    {
        
        /// <summary>
        /// Configure client to use MongoGatewayListProvider
        /// </summary>
        public static IClientBuilder UseApiClustering(this IClientBuilder builder, Action<ApiMembershipConfig> configurator = null)
        {
            return builder.ConfigureServices(services => services.AddApiProvider<SvcClientFactory>(configurator));
        }

        public static IClientBuilder UseApiClustering<TSvcClientFactory>(this IClientBuilder builder, Action<ApiMembershipConfig> configurator = null) 
            where TSvcClientFactory : class, ISvcClientFactory
        {
            return builder.ConfigureServices(services => services.AddApiProvider<TSvcClientFactory>(configurator));
        }


        internal static IServiceCollection AddApiProvider<TSvcClientFactory>(this IServiceCollection services,
            Action<ApiMembershipConfig> configurator = null) where TSvcClientFactory : class, ISvcClientFactory
        {
            services.Configure(configurator ?? (x => { }));
            services.AddSingleton<IGatewayListProvider, ApiGatewayListProvider>();
            services.AddSingleton<ISvcClientFactory, TSvcClientFactory>();

            return services;
        }

    }
}
