
using CommunAxiom.Commons.Client.Hosting.Operator.Services;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1;
using CommunAxiom.Commons.Client.Hosting.Operator.V1Alpha1.Entities;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.DotnetSdk.Helpers.OIDC;
using k8s.Models;
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
            services.Configure<OIDCSettings>(x => _configuration.GetSection("OIDC").Bind(x));
            services.AddTransient<OIDCSettings>(x => {
                var s =  new OIDCSettings();
                _configuration.GetSection("OIDC").Bind(s);
                return s;
            });

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

            var cl = new KubernetesClient();
            try
            {
                var mutator = cl.Get<V1MutatingWebhookConfiguration>("mutators.communaxiomcommonsagentoperator").GetAwaiter().GetResult();
                var validator = cl.Get<V1ValidatingWebhookConfiguration>("validators.communaxiomcommonsagentoperator").GetAwaiter().GetResult();
                cl.Delete(mutator).GetAwaiter().GetResult();
                cl.Delete(validator).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            services.AddSingleton<IKubernetesClient>(cl);
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

            services.AddHostedService<ConfigurationsMonitor>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseKubernetesOperator();
        }
    }
}
