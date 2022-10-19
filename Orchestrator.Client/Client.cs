using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using Comax.Commons.Orchestrator.Contracts.UriRegistry;

namespace Comax.Commons.Orchestrator.Client
{
    public class Client : IOrchestratorClient, IDisposable
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

        public IMailbox GetMailbox(string id)
        {
            return _clusterClient.GetGrain<IMailbox>(id);
        }

        public IUriRegistry GetUriRegistry(string id)
        {
            return _clusterClient.GetGrain<IUriRegistry>(id);
        }

        public IEventMailbox GetEventMailbox(Guid id)
        {
            return _clusterClient.GetGrain<IEventMailbox>(id);
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
