



using CommunAxiom.Commons.Client.ClusterClient;
using CommunAxiom.Commons.Client.DevSilo.System;
using CommunAxiom.Commons.Client.SiloShared;
using CommunAxiom.Commons.Client.SiloShared.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

Console.WriteLine("Starting service...");
var host = Host.CreateDefaultBuilder(args)
    .SetConfiguration(out var cfg, "./config.json")
    .ConfigureServices((host, sc) =>
    {
        sc.AddLogging(lb => lb.AddConsole());
        sc.SetServerServices();
        sc.SetupOrleansClient("./config.json");
        sc.AddSingleton<IClusterManagement, ClusterManagement>();
    }).Build();
var cm = host.Services.GetService<IClusterManagement>();

host.Run();
