using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.StorageGrain
{
    public class Repo
    {
        private readonly IPersistentState<Dictionary<string, JObject>> _state;
        public Repo(IPersistentState<Dictionary<string, JObject>> state)
        {
            _state = state;
        }

        public async Task<Dictionary<string, JObject>> Fetch()
        {
            await _state.ReadStateAsync();
            return _state.State;
        }

        public async Task Add(string key, JObject value)
        {
            var state = new Dictionary<string, JObject>();
            state.Add(key, value);
            _state.State = state;
            await _state.WriteStateAsync();
        }
    }
}
