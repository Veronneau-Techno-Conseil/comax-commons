using ClientUI.Components;
using CommunAxiom.Commons.ClientUI.Shared.Extensions;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;

namespace ClientUI.Client
{
    public static class WebAsmHost
    {
        public static async Task Launch(string[] args)
        {
            try
            {
                var builder = WebAssemblyHostBuilder.CreateDefault(args);
                builder.RootComponents.Add<App>("#app");
                builder.RootComponents.Add<HeadOutlet>("head::after");

                builder.Services.AddSingleton<IHostingEnvironment>(
                    new HostingEnvironment() { EnvironmentName = builder.HostEnvironment.Environment });

                builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
                var applicationSettingsSection = builder.Configuration;
                builder.Services.SetBlazorApp(applicationSettingsSection.Get<ApplicationSettings>());

                await builder.Build().RunAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
