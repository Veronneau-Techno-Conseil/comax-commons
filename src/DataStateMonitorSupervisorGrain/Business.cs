using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Grains.DataStateMonitor;
using CommunAxiom.Commons.Orleans;
using Orleans.Runtime;

namespace CommunAxiom.Commons.Client.Grains.DataStateMonitorSupervisorGrain
{
    public class Business
    {
        private Repo _repo;
        private readonly IComaxGrainFactory _grainFactory;

        public Business(IComaxGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        public void Init(IPersistentState<DataSateMonitorItem> keyState)
        {
            _repo = new Repo(keyState);
        }

        public Task RegisterAsync(string grainKey)
        {
            return _repo.AddAsync(grainKey);
        }

        public async Task UnregisterAsync(string grainKey)
        {
            await _repo.RemoveAsync(grainKey);
            var dataStateMonitor = _grainFactory.GetGrain<IDataStateMonitor>(grainKey);
            await dataStateMonitor.UnregisterAsync();
        }

        public async Task EnsureActiveAsync()
        {
            var items = await _repo.GetAsync();

            foreach (var grainKey in items.Keys)
            {
                var dataStateMonitor = _grainFactory.GetGrain<IDataStateMonitor>(grainKey);
                await dataStateMonitor.EnsureActive();
            }
        }
    }
}