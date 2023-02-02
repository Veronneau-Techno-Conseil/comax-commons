using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using DotnetKubernetesClient;
using KubeOps.Operator;
using Microsoft.Extensions.Logging.Console;

namespace CommunAxiom.Commons.Client.Hosting.Operator
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
#if DEBUG
                builder.AddSimpleConsole(opts =>
                {
                    opts.SingleLine = true;
                    opts.ColorBehavior = LoggerColorBehavior.Enabled;
                    opts.IncludeScopes = true;
                    opts.UseUtcTimestamp = true;
                    opts.TimestampFormat = "R";
                });
#else
            builder.AddJsonConsole(opts =>
            {
                opts.UseUtcTimestamp = true;
                opts.IncludeScopes = false;
                opts.TimestampFormat = "R";
            });

#endif
            });
            services.AddSingleton<IKubernetesClient>(new KubernetesClient());
            var operatorBuilder = services.AddKubernetesOperator();

            operatorBuilder
#if DEBUG
                .AddWebhookLocaltunnel()
#endif
                .AddEntity<AgentReferee>()
                .AddController<RefereeController>()
                .AddFinalizer<RefereeFinalizer>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseKubernetesOperator();
        }
    }
}
