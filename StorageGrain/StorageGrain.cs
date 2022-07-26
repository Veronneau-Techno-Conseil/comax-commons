using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.StorageGrain
{
    public class StorageGrain : Grain, IStorageGrain
    {
        private readonly IPersistentState<Dictionary<string, JObject>> _storageState;
        private readonly Repo _repo;
        public StorageGrain([PersistentState("storageGrain", Constants.Storage.JObjectStore)] IPersistentState<Dictionary<string, JObject>> storageState)
        {
            _storageState = storageState;
            _repo = new Repo(storageState);
        }

        public Task<Dictionary<string, JObject>> GetData()
        {
            return _repo.Fetch();
        }

        public Task SaveData(JObject value)
        {
            return _repo.Add(this.IdentityString, value);
        }
    }
}
