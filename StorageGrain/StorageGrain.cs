using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StorageGrain
{
    // HACK: 
    // [StorageProvider()]
    public class StorageGrain : IStorageGrain
    {
        private readonly IPersistentState<JObject> _storageState;

        public StorageGrain([PersistentState("storageGrain")] IPersistentState<JObject> storageState)
        {
            _storageState = storageState;
        }

        public Task<List<JObject>> GetData()
        {
            throw new System.NotImplementedException();
        }

        public Task SaveData(JObject obj)
        {
            _storageState.State = obj;
            _ =  _storageState.WriteStateAsync();
            return Task.CompletedTask;
        }
    }
}
