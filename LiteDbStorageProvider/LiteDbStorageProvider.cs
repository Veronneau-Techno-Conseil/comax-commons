using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace LiteDbStorageProvider
{
    public class LiteDbStorageProvider : Orleans.Storage.IStorageProvider
    {
        private readonly ILogger<LiteDbStorageProvider> logger;
        private readonly IOptionsMonitor<LiteDbConfig> configMonitor;
        public LiteDbStorageProvider(ILogger<LiteDbStorageProvider> logger, IOptionsMonitor<LiteDbConfig> configMonitor)
        {
            this.configMonitor = configMonitor;
            this.logger = logger;
        }
        public string Name { get; set; }
        
        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            
            throw new NotImplementedException();
        }

        public Task Close()
        {
            throw new NotImplementedException();
        }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            throw new NotImplementedException();
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }

        public Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new NotImplementedException();
        }
    }
}
