using CommunAxiom.DotnetSdk.Helpers.OIDC;
using GrainStorageService.Models.Configurations;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace GrainStorageService
{
    
    public static class Application
    {
        public static async Task<IHost> CreateApp(string configSource, params string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile(configSource)
                        .AddCommandLine(args)
                        .AddEnvironmentVariables();
            var _config = configurationBuilder.Build();
            AppConfig appConfig = new AppConfig();
            _config.Bind(appConfig);

            var _hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(wb =>
                {
                    wb.UseKestrel(opts =>
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

                    wb.Configure(app =>
                    {
                        Console.WriteLine($"Migrate db...");
                        app.ApplicationServices.MigrateDb();

#if DEBUG
                        Console.WriteLine("Launching mariadb in debug");
                        var dbConf = app.ApplicationServices.GetService<IOptionsMonitor<GrainStorageService.DbConf>>();
                        var curconf = dbConf.CurrentValue;
                        try
                        {
                            DockerIntegration.Client client = new DockerIntegration.Client();
                            client.InstallContainer("mariadbdevstorage", "mariadb", "10.10",
                                new Dictionary<string, string> { { "3306/tcp", curconf.Port } },
                                new List<string> { $"MYSQL_ROOT_PASSWORD={curconf.Password}" }).ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
#endif

                        app.UseSwagger();
                        app.UseSwaggerUI();
                        // Configure the HTTP request pipeline.
                        //if (!app.Environment.IsDevelopment())
                        //{
                        app.UseExceptionHandler("/Error");
                        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                        app.UseHsts();
                        
                        app.UseHttpsRedirection();
                        app.UseStaticFiles();

                        app.UseRouting();

                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.UseEndpoints(arb =>
                        {
                            arb.MapControllers();
                            arb.MapRazorPages();
                        });
                    });
                    
                })
                .ConfigureAppConfiguration(c => c.AddJsonFile(configSource)
                        .AddCommandLine(args)
                        .AddEnvironmentVariables())
                .ConfigureServices(sc =>
                {
                    sc.AddEndpointsApiExplorer();
                    sc.AddSwaggerGen();
                    // Add services to the container.
                    sc.AddRazorPages();
                    // Add services to the container.
                    sc.Configure<GrainStorageService.DbConf>(x => _config.GetSection("DbConfig").Bind(x));
                    sc.AddDbContext<GrainStorageService.Models.StorageDbContext>();

                    Console.WriteLine($"Setting services...");
                    sc.Configure<OIDCSettings>(x => _config.GetSection("OIDC").Bind(x));

                });

            //}

            var app = _hostBuilder.Build();
            return app;
        }
    }
}
