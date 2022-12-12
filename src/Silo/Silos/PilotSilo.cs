using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Auth;
using CommunAxiom.Commons.Client.Grains.AccountGrain;
using CommunAxiom.Commons.Client.SiloShared;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    //public static class PilotSilo
    //{
    //    static ISiloHost _silo;
    //    public static ISiloHost Host { get { return _silo; } }
    //    public static bool IsSiloStarted { get; private set; }

    //    public static async Task StartSilo()
    //    {
    //        if (IsSiloStarted)
    //        {
    //            return;
    //        }
    //        // define the cluster configuration
    //        var builder = new SiloHostBuilder()
    //            .SetDefaults(out var conf)
    //            .ConfigureServices(services =>
    //            {
    //                services.SetStorage(conf);
    //                services.AddScoped<AccountRepo, AccountRepo>();
    //                services.AddScoped<AccountBusiness, AccountBusiness>();
    //                services.AddSingleton<IAccount, Accounts>();
    //                services.AddSingleton<IAuthentication, AuthenticationWorker>();

    //            })
    //            //configure application parts for each grain
    //            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Accounts).Assembly).WithReferences());

    //        var silo = builder.Build();
    //        await silo.StartAsync();
    //        _silo = silo;
    //        IsSiloStarted = true;
    //    }

    //    public static async Task StopSilo()
    //    {
    //        if (IsSiloStarted)
    //        {
    //            await _silo.StopAsync();
    //            await _silo.DisposeAsync();
    //            _silo = null;
    //            IsSiloStarted=false;
    //        }
    //    }
    //}
}
