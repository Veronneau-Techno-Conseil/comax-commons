using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Comax.Commons.Orchestrator.CentralGrain;

namespace Comax.Commons.Orchestrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting service...");
            var host = Host.CreateDefaultBuilder(args)
                .SetConfiguration(out var cfg)
                .ConfigureServices((host, sc) =>
                {
                    sc.SetServerServices(cfg);
                    sc.AddLogging(lb => lb.AddConsole());
                }).Build();

            host.Run();
        }
    }
}
