using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.CommonsShared.ApiMembershipProvider
{
    public static class SiloExtensions
    {
        public static IServiceCollection UseApiMembership(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiMembershipConfig>(configuration);
            services.AddSingleton<ISvcClientFactory, SvcClientFactory>();
            services.AddSingleton<IMembershipTable, ApiMembershipProvider.ApiMembershipTableProvider>();
            return services;
        }
    }
}
