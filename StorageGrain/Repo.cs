using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.StorageGrain
{
    public class Repo
    {
        private readonly IPersistentState<JObject> _state;
        public Repo(IPersistentState<JObject> state)
        {
            _state = state;
        }

        public async Task<JObject> Fetch()
        {
            await _state.ReadStateAsync();
            return _state.State;
        }

        public async Task Save(JObject value)
        {

            _state.State = value;
            await _state.WriteStateAsync();
        }
    }
}
