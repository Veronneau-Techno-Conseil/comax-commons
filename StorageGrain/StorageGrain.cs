using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using CommunAxiom.Commons.Orleans;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.StorageGrain
{
    public class StorageGrain : Grain, IStorageGrain
    {
        private readonly IPersistentState<JObject> _storageState;
        private readonly Repo _repo;
        public StorageGrain([PersistentState("storageGrain", Constants.Storage.JObjectStore)] IPersistentState<JObject> storageState)
        {
            _storageState = storageState;
            _repo = new Repo(storageState);
        }

        public Task<JObject> GetData()
        {
            return _repo.Fetch();
        }

        public Task SaveData(JObject value)
        {
            return _repo.Save(value);
        }
    }
}
