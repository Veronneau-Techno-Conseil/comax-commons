using CommunAxiom.Commons.Client.Hosting.Operator.Models;
using CommunAxiom.Commons.Client.Hosting.Operator.Services;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using KubeOps.KubernetesClient;
//using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;

using KubeOps.Operator;
using Microsoft.Extensions.Logging.Console;

namespace CommunAxiom.Commons.Client.Hosting.Operator
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OIDCConfig>(_configuration.GetSection("OIDC"));
            services.AddHostedService<ConfigurationsMonitor>();

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
                
                
                .AddEntity<AgentSilo>()
                .AddController<AgentSiloController>()
                .AddFinalizer<AgentSiloFinalizer>()

                .AddEntity<ComaxAgent>()
                .AddController<ComaxAgentController>()
                .AddFinalizer<ComaxAgentFinalizer>()

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
