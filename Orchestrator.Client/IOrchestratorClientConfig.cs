using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Client
{
    public interface IOrchestratorClientConfig
    {
        void Configure(IServiceCollection sc);
    }
}
