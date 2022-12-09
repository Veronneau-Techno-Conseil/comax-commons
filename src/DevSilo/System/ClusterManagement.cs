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
    public class ClusterManagement : IClusterManagement
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger; 
        //private SystemListener systemListener;
        private readonly IConfiguration configuration;
        public ClusterManagement(IServiceProvider serviceProvider, ILogger<ClusterManagement> logger, IConfiguration configuration)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.configuration = configuration;
        }

        public SiloShared.System.Silos CurrentSilo { get; set; } = SiloShared.System.Silos.Unspecified;

        public async Task Heartbeat()
        {
            if(CurrentSilo == SiloShared.System.Silos.Unspecified)
            {
                //systemListener = new SystemListener(this, serviceProvider.GetRequiredService<ICommonsClientFactory>(), serviceProvider.GetRequiredService<IConfiguration>());
                //await systemListener.Listen();
                _ = SetSilo(SiloShared.System.Silos.Main);
            }
        }

        public Task<bool> IsServiceAuthSet()
        {
            return Task.FromResult(true);
        }

        public async Task SetSilo(SiloShared.System.Silos requiredSilo)
        {
            CurrentSilo = requiredSilo;
            await Silos.DevSilo.StartSilo();
        }
    }
}
