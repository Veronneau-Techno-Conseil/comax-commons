using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Auth;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Project;
using CommunAxiom.Commons.Client.Contracts.Replication;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Grains.DateStateMonitorSupervisor;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using Microsoft.Extensions.Logging;
using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.Client.Contracts.Grains.Explorer;

namespace CommunAxiom.Commons.Client.ClusterClient
{
    public class Client : ICommonsClusterClient, IDisposable
    {
        private bool _disposed = false;
        private readonly IClusterClient _clusterClient;
        private readonly ILogger _logger;
        private Guid? _userID = null;

        public Client(IClusterClient clusterClient, ILogger<Client> logger)
        {
            _clusterClient = clusterClient;
            _logger = logger;
        }

        ~Client()
        {
            if (!_disposed) { Dispose(); }
        }

        public IDateStateMonitorSupervisor GetDateStateMonitorSupervisor()
        {
            return _clusterClient.GetGrain<IDateStateMonitorSupervisor>(Guid.Empty);
        }

        public IAccount GetAccount()
        {
            return _clusterClient.GetGrain<IAccount>(Guid.Empty);
        }

        public IAgent GetAgent()
        {
            return _clusterClient.GetGrain<IAgent>(Guid.Empty);
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

        public IPortfolio GetPortfolio()
        {
            return _clusterClient.GetGrain<IPortfolio>(Guid.Empty);
        }

        public IProject GetProject(string projectId)
        {
            return _clusterClient.GetGrain<IProject>(projectId);
        }

        public IReplication GetReplication()
        {
            return _clusterClient.GetGrain<IReplication>(Guid.Empty);
        }

        public IScheduler GetScheduler(int schedulerID)
        {
            return _clusterClient.GetGrain<IScheduler>(schedulerID);
        }

        public IExplorer GetExplorer()
        {
            return _clusterClient.GetGrain<IExplorer>(Guid.Empty);
        }

        public async Task<StreamSubscriptionHandle<SystemEvent>> SubscribeSystem(Func<SystemEvent, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted)
        {
            var res = await _clusterClient.GetStreamProvider(OrleansConstants.Streams.DefaultStream)
                    .GetStream<SystemEvent>(Guid.Empty, OrleansConstants.StreamNamespaces.DefaultNamespace)
                    .SubscribeAsync(fn, funcError, onCompleted);
            return res;
        }

        public async Task<(StreamSubscriptionHandle<AuthorizationInstructions>, AsyncEnumerableStream<AuthorizationInstructions>)> SubscribeAuth(Guid streamId)
        {
            var result = new AsyncEnumerableStream<AuthorizationInstructions>();
            var handle = await _clusterClient.GetStreamProvider(OrleansConstants.Streams.DefaultStream)
                    .GetStream<AuthorizationInstructions>(streamId, OrleansConstants.StreamNamespaces.DefaultNamespace)
                    .SubscribeAsync(result);
            return (handle, result);
        }

        public IUriRegistry GetUriRegistry(string uri)
        {
            string actual = OrleansConstants.BLANK_ID;
            if (!string.IsNullOrWhiteSpace(uri))
            {
                if (!uri.StartsWith("usr://") && !uri.StartsWith("com://"))
                {
                    throw new ArgumentException("Id should be a uri");
                }
                actual = uri;
            }
            return _clusterClient.GetGrain<IUriRegistry>(actual);
        }

        private async Task<Guid> LoadUserId()
        {
            if (_userID == null)
                _userID = await this.GetUriRegistry("").GetOrCreate();
            return _userID.Value;
        }

        public async Task<IEventMailboxClient> GetEventMailbox(Guid? id = null)
        {
            Guid? userID = id;
            if (userID == null)
                userID = await this.LoadUserId();
            var gr = _clusterClient.GetGrain<IEventMailbox>(userID.Value);
            var cl = new EventMailboxClient(_clusterClient, gr, _logger);
            return cl;
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

        public IStreamProvider GetStreamProvider(string name)
        {
            return _clusterClient.GetStreamProvider(name);
        }
    }
}
