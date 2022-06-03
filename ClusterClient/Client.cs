using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Auth;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Replication;
using CommunAxiom.Commons.Shared;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterClient
{
    public class Client : ICommonsClusterClient, IDisposable
    {
        private bool _disposed = false;
        private readonly IClusterClient _clusterClient;
        public Client(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        ~Client()
        {
            if (!_disposed) { Dispose(); }
        }

        public IAccount GetAccount()
        {
            return _clusterClient.GetGrain<IAccount>(Guid.Empty);
        }

        public IAuthentication GetAuthentication()
        {
            return _clusterClient.GetGrain<IAuthentication>(Guid.Empty);
        }

        public IDatasource GetDatasource(string datasourceId)
        {
            return _clusterClient.GetGrain<IDatasource>(datasourceId);
        }

        public IDataTransfer GetDataTransfer(string operationId)
        {
            return _clusterClient.GetGrain<IDataTransfer>(operationId);
        }

        public IIngestion GetIngestion(string ingestionId)
        {
            return _clusterClient.GetGrain<IIngestion>(ingestionId);
        }

        public IPortfolio GetPortfolio(string portfolioId)
        {
            return _clusterClient.GetGrain<IPortfolio>(portfolioId);
        }

        public IProject GetProject(string projectId)
        {
            return _clusterClient.GetGrain<IProject>(projectId);
        }

        public IReplication GetReplication()
        {
            return _clusterClient.GetGrain<IReplication>(Guid.Empty);
        }

        public async Task<StreamSubscriptionHandle<SystemEvent>> SubscribeSystem(Func<SystemEvent, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted)
        {
            var res = await _clusterClient.GetStreamProvider(Constants.DefaultStream)
                    .GetStream<SystemEvent>(Guid.Empty, Constants.DefaultNamespace)
                    .SubscribeAsync(fn, funcError, onCompleted);
            return res;
        }

        public async Task<(StreamSubscriptionHandle<AuthorizationInstructions>, AsyncEnumerableStream<AuthorizationInstructions>)> SubscribeAuth(Guid streamId)
        {
            var result = new AsyncEnumerableStream<AuthorizationInstructions>();
            var handle = await _clusterClient.GetStreamProvider(Constants.DefaultStream)
                    .GetStream<AuthorizationInstructions>(streamId, Constants.DefaultNamespace)
                    .SubscribeAsync(result);
            return (handle, result);
        }

        public void Dispose()
        {
            try
            {
                _clusterClient.Close().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {

            }
            _clusterClient.Dispose();
            _disposed = true;
        }

        public Task Close()
        {
            return _clusterClient.Close();
        }
    }
}
