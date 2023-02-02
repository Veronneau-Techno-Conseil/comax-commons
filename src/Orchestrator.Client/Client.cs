using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.Central;
using CommunAxiom.Commons.Shared.RuleEngine;
using Comax.Commons.Orchestrator.Contracts.SOI;
using System.IO;
using CommunAxiom.Commons.Orleans;
using System.Xml;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using Comax.Commons.Orchestrator.Contracts.PublicBoard;
using Microsoft.Extensions.Logging;
using Comax.Commons.Orchestrator.Contracts.CommonsActor;
using Comax.Commons.Orchestrator.Contracts.Portfolio;

namespace Comax.Commons.Orchestrator.Client
{
    public class Client : IOrchestratorClient, IDisposable
    {
        private bool _disposed = false;
        private readonly IClusterClient _clusterClient;
        private readonly ILogger<Client> _logger;
        private readonly HandlerToDelegateRelay _onDisconnectRelay;
        private bool _connected = true;

        private UserTuple? _currentUser = null;
        public Client(IClusterClient clusterClient, ILogger<Client> logger, HandlerToDelegateRelay onDisconnectRelay)
        {
            _clusterClient = clusterClient;
            _logger = logger;
            _onDisconnectRelay = onDisconnectRelay;
            _onDisconnectRelay.Delegate = (o, args) => this._connected = false;
        }

        ~Client()
        {
            if (!_disposed) { Dispose(); }
        }

        public bool IsConnected
        {
            get
            {
                return this._connected;
            }
        }

        public IClusterClient ClusterClient
        {
            get
            {
                return _clusterClient;
            }
        }


        private async Task<UserTuple> LoadUser()
        {
            if (_currentUser == null)
                _currentUser = await this.GetUriRegistry("").GetCurrentUser();
            return _currentUser;
        }

        public IUriRegistry GetUriRegistry(string id)
        {
            string actual = OrleansConstants.BLANK_ID;
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (!id.StartsWith("usr://") && !id.StartsWith("com://"))
                {
                    throw new ArgumentException("Id should be a uri");
                }
                actual = id;
            }
            return _clusterClient.GetGrain<IUriRegistry>(actual);
        }

        public async Task<IEventMailboxClient> GetEventMailbox(Guid? id = null)
        {
            Guid? userID = id;
            if(userID == null)
                userID = (await this.LoadUser()).InternalId;
            var gr = _clusterClient.GetGrain<IEventMailbox>(userID.Value);
            var cl = new EventMailboxClient(ClusterClient, gr, _logger);
            return cl;
        }

        public async Task<ICommonsActor> GetActor(string uri = null)
        {
            var uriValue = uri ?? (await this.LoadUser()).Uri;
            var gr = _clusterClient.GetGrain<ICommonsActor>(uriValue);
            return gr;
        }

        public ICentral GetCentral()
        {
            return _clusterClient.GetGrain<ICentral>(Guid.Empty);
        }

        public IPortfolioRegistry GetPortfolioRegistry()
        {
            return _clusterClient.GetGrain<IPortfolioRegistry>(Guid.Empty);
        }

        public void Dispose()
        {
            try
            {
                _clusterClient.Close().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            _clusterClient.Dispose();
            _disposed = true;
        }

        public Task Close()
        {
            return _clusterClient.Close();
        }

        public IPublicBoard GetPublicBoard()
        {
            throw new NotImplementedException();
        }

        public async Task<ISubjectOfInterest> GetSubjectOfInterest()
        {
            Guid userID = (await this.LoadUser()).InternalId;
            return _clusterClient.GetGrain<ISubjectOfInterest>(userID);
        }
    }
}
