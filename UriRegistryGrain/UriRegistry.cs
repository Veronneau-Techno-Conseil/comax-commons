using Orleans;
using Comax.Commons.Orchestrator.Contracts.UriRegistry;
using System.Threading.Tasks;
using Orleans.Runtime;
using System;

namespace Comax.Commons.Orchestrator.UriRegistryGrain
{
    public class UriRegistry: Grain, IUriRegistry
    {
        private IPersistentState<IdContainer> _storageState;
        public UriRegistry([PersistentState("uriregistry")] IPersistentState<IdContainer> storageState)
        {
            _storageState = storageState;
        }
        
        public async Task<Guid> GetOrCreate()
        {
            await _storageState.ReadStateAsync();

            if(string.IsNullOrWhiteSpace(_storageState.State?.Guid))
            {
                _storageState.State = new IdContainer
                {
                    Guid = Guid.NewGuid().ToString()
                };
                await _storageState.WriteStateAsync();
            }
            return Guid.Parse(_storageState.State.Guid);
        }

    }
}
