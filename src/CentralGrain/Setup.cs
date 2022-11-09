using Comax.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CommunAxiom.Commons.CommonsShared.CentralGrain
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection CentralGrainSetup(this IServiceCollection sc)
        {
            sc.AddScoped<SvcFactory>();
            return sc;
        }
    }
}
