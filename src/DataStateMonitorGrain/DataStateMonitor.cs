using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Grains.DataStateMonitor;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Orleans;
using Orleans;
using Orleans.GrainDirectory;

namespace CommunAxiom.Commons.Client.Grains.DataStateMonitorGrain
{
    [GrainDirectory(GrainDirectoryName = "MyGrainDirectory")]
    public class DataStateMonitor : Grain, IDataStateMonitor, IDisposable
    {
        private IDisposable _dataStateTimer = null!;
        private readonly IDataSourceFactory _dataSourceFactory;
        private bool _disposedValue;

        public DataStateMonitor(IDataSourceFactory dataSourceFactory)
        {
            _dataSourceFactory = dataSourceFactory;
        }

        public override Task OnActivateAsync()
        {
            _dataStateTimer = RegisterTimer(
                x => Execute(),
                true,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(1)
            ); //Repeat each second to make sure cover every cron job

            return Task.FromResult(base.OnActivateAsync());
        }

        public async Task Execute()
        {
            var dataStateBusiness = new Business(
                new GrainFactory(this.GrainFactory),
                GrainReference.GrainIdentity.PrimaryKeyString, _dataSourceFactory
            );

            await dataStateBusiness.Execute();
        }

        public Task UnregisterAsync()
        {
            Dispose(true);
            return Task.CompletedTask;
        }

        public Task EnsureActive()
        {
            return Task.CompletedTask;
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