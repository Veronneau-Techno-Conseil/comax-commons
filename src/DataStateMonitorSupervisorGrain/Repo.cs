using System.Threading.Tasks;
using Orleans.Runtime;
using System.Linq;

namespace CommunAxiom.Commons.Client.Grains.DataStateMonitorSupervisorGrain
{ 
    public class Repo
    {
        private readonly IPersistentState<DataSateMonitorItem> _keysState;

        public Repo(IPersistentState<DataSateMonitorItem> keysState)
        {
            _keysState = keysState;
        }

        public async Task AddAsync(string grainKey)
        {
            await _keysState.ReadStateAsync();
            _keysState.State ??= new DataSateMonitorItem();
            
            if (_keysState.State.Keys.All(x => x != grainKey))
            {
                _keysState.State.Keys.Add(grainKey);
                await _keysState.WriteStateAsync();
            }
        }

        public async Task RemoveAsync(string grainKey)
        {
            await _keysState.ReadStateAsync();

            if (_keysState.State != null)
            {
                _keysState.State.Keys.Remove(grainKey);
                await _keysState.WriteStateAsync();
            }
        }

        public async Task<DataSateMonitorItem> GetAsync()
        {
            await _keysState.ReadStateAsync();
            return _keysState.State;
        }
    }
}