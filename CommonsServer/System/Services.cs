using ClusterClient;
using CommunAxiom.Commons.Client.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo.System
{
    public static class Services
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        private static ICommonsClusterClient _commonsClusterClient;
        public static ICommonsClusterClient CommonsClusterClient 
        {
            get
            {
                if (_commonsClusterClient == null)
                {
                    _commonsClusterClient = _commonsClusterClient ?? ServiceProvider.GetService<ICommonsClusterClient>();
                }
                return _commonsClusterClient;
            }
        }

        public static ClusterManagement ClusterManagement { get; private set; }

        public static void Bootstrap()
        {
            SetServices();
            ClusterManagement = ServiceProvider.GetService<ClusterManagement>();
        }

        private static void SetServices()
        {
            var sc = new ServiceCollection();
            sc.SetupOrleansClient();
            sc.AddSingleton<ClusterManagement>();
            sc.AddLogging(lb => lb.AddConsole());
            ServiceProvider = sc.BuildServiceProvider();
        }

        public static void ResetClient()
        {
            _commonsClusterClient = null;
        }

    }
}
