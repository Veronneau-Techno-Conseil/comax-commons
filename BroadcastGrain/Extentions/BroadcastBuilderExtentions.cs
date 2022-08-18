using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.DependencyInjection;
using Neleus.DependencyInjection.Extensions;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public static class BroadcastBuilderExtentions
    {
        public static void AddIngestion(this IServiceCollection services)
        {
            services.AddTransient<ExecutorTargets>();
            services.AddByName<IExecutor<Message>>().Add("ExecutorTargets", typeof(IngestionStartedExecutor)).Build();
        }
    }
}
