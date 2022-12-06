using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace CommunAxiom.Commons.Client.Grains.DateStateMonitorSupervisorGrain
{
    public class Repo
    {
        private readonly IPersistentState<List<string>> _keysState;

        public Repo(IPersistentState<List<string>> keysState)
        {
            _keysState = keysState;
        }

        public async Task AddAsync(string grainKey)
        {
            await _keysState.ReadStateAsync();

            _keysState.State ??= new List<string>();

            _keysState.State.Add(grainKey);
        }

        public async Task RemoveAsync(string grainKey)
        {
            await _keysState.ReadStateAsync();

            if (_keysState.State != null)
            {
                _keysState.State.Remove(grainKey);
            }
        }

        public async Task<List<string>> GetAsync()
        {
            await _keysState.ReadStateAsync();
            return _keysState.State;
        }
    }
}