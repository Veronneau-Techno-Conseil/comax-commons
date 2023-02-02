using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.CommonsShared.UriRegistryGrain
{
    public class IdRegistry : Grain, IIdRegistry
    {
        private readonly IPersistentState<UserTuple> _storageState;
        public IdRegistry([PersistentState("userTuple")] IPersistentState<UserTuple> storageState) 
        {
            _storageState= storageState;
        }
        public async Task<UserTuple> Get()
        {
            await _storageState.ReadStateAsync();
            return _storageState.State;
        }

        public async Task Set(UserTuple value)
        {
            await _storageState.ReadStateAsync();
            _storageState.State = value;
            await _storageState.WriteStateAsync();
        }
    }
}
