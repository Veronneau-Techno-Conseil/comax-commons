



using ClusterClient;
using CommunAxiom.Commons.Client.DevSilo.System;
using CommunAxiom.Commons.Client.SiloShared;
using CommunAxiom.Commons.Client.SiloShared.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

Console.WriteLine("Starting service...");
var host = Host.CreateDefaultBuilder(args)
    .SetConfiguration(out var cfg)
    .ConfigureServices((host, sc) =>
    {
        sc.SetServerServices();
        sc.SetupOrleansClient();
        sc.AddTransient<IClusterManagement, ClusterManagement>();
        sc.AddLogging(lb => lb.AddConsole());
    }).Build();
var cm = host.Services.GetService<IClusterManagement>();

host.Run();
