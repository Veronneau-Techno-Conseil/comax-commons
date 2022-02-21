using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Account;
using ElectronNET.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace CommunAxiom.Commons.ClientUI
{
    public class Startup
    {
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            //Enable CORS to enable calling an API from JavaScript
            //Since we’re building an API that will be called by an Angular app, we need CORS. 
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader());
            //});

            // Add Orleans Service 
            //Consider refactoring the code
            services.AddSingleton<IClusterClient>(provider => {
                var hostingEnvironment = provider.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();

                var clientBuilder = new ClientBuilder()
                    .Configure<ClusterOptions>(options => {
                        options.ClusterId = "dev";
                        options.ServiceId = "CoreBlog";
                    })
                    .ConfigureApplicationParts(parts => {
                        parts.AddFromApplicationBaseDirectory();
                    });

                if (hostingEnvironment.IsDevelopment())
                {
                    clientBuilder = clientBuilder.UseLocalhostClustering();
                }

                var client = clientBuilder.Build();
                var reset = new ManualResetEvent(false);

                client.Connect(RetryFilter).ContinueWith(task => {
                    reset.Set();

                    return Task.CompletedTask;
                });

                reset.WaitOne();

                return client;

                async Task<bool> RetryFilter(Exception exception)
                {
                    provider.GetService<ILogger>()?.LogWarning(
                        exception,
                        "Exception while attempting to connect to Orleans cluster"
                    );

                    await Task.Delay(TimeSpan.FromSeconds(2));

                    return true;
                }
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //To allow any API request from any origin
            //To allow for certain controllers or methods add theEnableCors attribute to specific controllers or methods
            //app.UseCors("CorsPolicy");


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            Task.Run(async () =>
            {
                var window = await Electron.WindowManager.CreateWindowAsync();
                window.SetMenu(null);
            });
        }
    }
}
