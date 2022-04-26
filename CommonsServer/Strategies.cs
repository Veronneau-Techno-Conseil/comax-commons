using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public static class Strategies
    {
        public static ISiloHostBuilder SetClustering(this ISiloHostBuilder siloHostBuilder)
        {
            return siloHostBuilder.UseLocalhostClustering();
        }

        public static ISiloHostBuilder SetConfiguration(this ISiloHostBuilder siloHostBuilder)
        {
            return siloHostBuilder.ConfigureAppConfiguration(app =>
            {
                app.AddJsonFile("./app.json");
            });
        }
    }
}
