using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Grains.DateStateMonitorSupervisor;
using CommunAxiom.Commons.Orleans;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;

namespace CommunAxiom.Commons.Client.Grains.DataStateMonitorSupervisorGrain
{
    [Reentrant]
    public class DataStateMonitorSupervisor : Grain, IDataStateMonitorSupervisor, IDisposable
    {
        private IDisposable _dataStateTimer = null!;
        
        // HACK: make sure this property serilize properly.
        private readonly IPersistentState<DataSateMonitorItem> _keys;
        
        private Business _business;
        private bool _disposedValue;

        public DataStateMonitorSupervisor([PersistentState("DateStateMonitorKeys")] IPersistentState<DataSateMonitorItem> keys)
        {
            _keys = keys;
        }

        public override Task OnActivateAsync()
        {
            _business = new Business(new GrainFactory(this.GrainFactory, this.GetStreamProvider));
            _business.Init(_keys);
            
            _dataStateTimer = RegisterTimer(
                x => _business.EnsureActiveAsync(),
                true,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(1)
            ); // remark: 1 minute for PROD - 1 seconds for TEST - This setting will be configured from adminstration page

            return Task.FromResult(base.OnActivateAsync());
        }

        public Task RegisterAsync(string grainKey)
        {
            return _business.RegisterAsync(grainKey);
        }

        public Task UnregisterAsync(string grainKey)
        {
            return _business.UnregisterAsync(grainKey);
        }
        
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _dataStateTimer.Dispose();
            }

            _disposedValue = true;
        }
        
    }
}