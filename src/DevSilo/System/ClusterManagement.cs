using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.SiloShared;
using CommunAxiom.Commons.Client.SiloShared.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.DevSilo.System
{
    public class ClusterManagement : IClusterManagement, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly Silos.DevSilo devSilo;
        public ClusterManagement(IServiceProvider serviceProvider, ILogger<ClusterManagement> logger, IConfiguration configuration)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.configuration = configuration;
            this.devSilo = new Silos.DevSilo();
        }

        public bool SiloStarted { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Heartbeat()
        {
            if (!SiloStarted)
                _ = StartSilo();
        }

        public Task<bool> IsServiceAuthSet()
        {
            return Task.FromResult(true);
        }

        public async Task StartSilo()
        {
            await devSilo.StartSilo();
            SiloStarted = true;
        }
    }
}
