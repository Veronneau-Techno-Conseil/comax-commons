using OpenIddict.Validation.AspNetCore;
using Orleans;
using System.Security.Cryptography.X509Certificates;
using CommunAxiom.CentralApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Comax.Commons.Orchestrator.MembershipProvider;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.CommonsShared.MembershipApi
{
    public abstract class Application
    {
        private readonly string _configSource;
        protected IHostBuilder _hostBuilder;
        protected IConfiguration _config;

        public Application(string configSource)
        {
            _configSource = configSource;
        }

        public void Init(params string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile(_configSource)
                        .AddCommandLine(args)
                        .AddEnvironmentVariables();
            _config = configurationBuilder.Build();
            AppConfig appConfig = new AppConfig();
            _config.Bind(appConfig);

            _hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Console.WriteLine($"Using kestrel on {appConfig.ListenPort} over {(appConfig.UseHttps ? "https" : "http")}...");

                    webBuilder.UseKestrel(opts =>
                    {
                        if (appConfig.UseHttps)
                        {
                            opts.ConfigureHttpsDefaults(def =>
                            {
                                var certPem = File.ReadAllText("cert.pem");
                                var eccPem = File.ReadAllText("key.pem");

                                var cert = X509Certificate2.CreateFromPem(certPem, eccPem);
                                def.ServerCertificate = new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
                                def.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                            });

                            opts.ConfigureEndpointDefaults(lo =>
                            {
                                lo.UseHttps();
                            });
                        }
                        opts.ListenAnyIP(appConfig.ListenPort);

                    });

                    webBuilder.Configure((app) =>
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI();

                        if (appConfig.UseHttps)
                        {
                            app.UseHttpsRedirection();
                        }

                        app.UseAuthentication();
                        app.UseRouting();
                        app.UseAuthorization();
                        app.UseEndpoints(endpoints => endpoints.MapControllers());
                    });
                })
                .ConfigureAppConfiguration(app =>
                    app.AddJsonFile(_configSource)
                        .AddCommandLine(args)
                        .AddEnvironmentVariables())
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureServices(services =>
                {
                    services.Configure<OidcConfig>(x => _config.GetSection("OIDC").Bind(x));

                    // Add services to the container.
                    services.AddControllers()
                    .AddJsonOptions(x =>
                    {
                        x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    });
                    services.AddEndpointsApiExplorer();
                    services.AddSwaggerGen();

                    // Register the OpenIddict validation components.
                    services.AddOpenIddict()
                        .AddValidation(options =>
                        {
                            OidcConfig oidcConfig = new OidcConfig();
                            _config.GetSection("OIDC").Bind(oidcConfig);
                            // Note: the validation handler uses OpenID Connect discovery
                            // to retrieve the address of the introspection endpoint.
                            options.SetIssuer(oidcConfig.Authority);
                            //options.AddAudiences("contacts_oi");

                            // Configure the validation handler to use introspection and register the client
                            // credentials used when communicating with the remote introspection endpoint.
                            options.UseIntrospection()
                                   .SetClientId(oidcConfig.ClientId)
                                   .SetClientSecret(oidcConfig.Secret);

                            // Register the System.Net.Http integration.
                            options.UseSystemNetHttp();

                            // Register the ASP.NET Core host.
                            options.UseAspNetCore();
                        });

                    services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

                    services.AddAuthorization(opt =>
                    {
                        opt.AddPolicy("Authenticated", builder =>
                        {
                            AuthenticatedPolicyAssertion(builder);
                        });

                        opt.AddPolicy("Reader", builder =>
                        {
                            ReaderPolicyAssertion(builder);
                        });

                        opt.AddPolicy("Actor", builder =>
                        {
                            ActorPolicyAssertion(builder);
                        });
                    });
                    services.AddSingleton<Comax.Commons.Orchestrator.MembershipProvider.Models.IMembershipTable, MembershipTable>();
                });

            this.SetStorage();
        }

        public virtual void AuthenticatedPolicyAssertion(AuthorizationPolicyBuilder builder)
        {
            builder.RequireAssertion(ctxt =>
            {
                return ctxt.User != null;
            });
        }

        public virtual void ReaderPolicyAssertion(AuthorizationPolicyBuilder builder)
        {
            builder.RequireClaim("https://referee.communaxiom.org/reader", "access");
        }

        public virtual void ActorPolicyAssertion(AuthorizationPolicyBuilder builder)
        {
            builder.RequireAssertion(ctxt =>
            {
                return ctxt.User.HasClaim(x => x.Type == "https://referee.communaxiom.org/actor");
            });
        }

        public abstract void SetStorage();

        public void Run()
        {
            var app = _hostBuilder.Build();
            
            app.Run();
        }

        public Task RunAsync()
        {
            var app = _hostBuilder.Build();

            return app.RunAsync();
        }
    }
}
