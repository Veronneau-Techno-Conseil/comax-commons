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
using Comax.Commons.Orchestrator.Contracts.Central;
using CommunAxiom.Commons.Shared.RuleEngine;
using Comax.Commons.Orchestrator.Contracts.SOI;
using System.IO;
using CommunAxiom.Commons.Orleans;
using System.Xml;
using CommunAxiom.Commons.Shared;

namespace Comax.Commons.Orchestrator.Client
{
    public class Client : IOrchestratorClient, IDisposable
    {
        private bool _disposed = false;
        private readonly IClusterClient _clusterClient;

        private Guid? _userID = null;
        public Client(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        ~Client()
        {
            if (!_disposed) { Dispose(); }
        }

        public IClusterClient ClusterClient
        {
            get
            {
                return _clusterClient;
            }
        }


        private async Task<Guid> LoadUserId()
        {
            if (_userID == null)
                _userID = await this.GetUriRegistry("").GetOrCreate();
            return _userID.Value;
        }

        public IUriRegistry GetUriRegistry(string id)
        {
            string actual = Constants.BLANK_ID;
            if(!string.IsNullOrWhiteSpace(id))
                actual = id;
            return _clusterClient.GetGrain<IUriRegistry>(actual);
        }

        public async Task<IEventMailbox> GetEventMailbox(Guid? id = null)
        {
            Guid? userID = id;
            if(userID == null)
                userID = await this.LoadUserId();
            return _clusterClient.GetGrain<IEventMailbox>(userID.Value);
        }

        public ICentral GetCentral()
        {
            return _clusterClient.GetGrain<ICentral>(Guid.Empty);
        }
        public Task<StreamSubscriptionHandle<MailMessage>> SubscribeEventMailboxStream(Guid id, Func<MailMessage, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted)
        {
            return _clusterClient.GetStreamProvider(Constants.DefaultStream)
                    .GetStream<MailMessage>(id, Constants.DefaultNamespace).SubscribeAsync(fn,funcError,onCompleted);
        }

        public async Task<(StreamSubscriptionHandle<MailMessage>, AsyncEnumerableStream<MailMessage>)> EnumEventMailbox(Guid streamId)
        {
            var result = new AsyncEnumerableStream<MailMessage>();
            var handle = await _clusterClient.GetStreamProvider(Constants.DefaultStream)
                    .GetStream<MailMessage>(streamId, Constants.DefaultNamespace)
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
                Console.Error.WriteLine(ex);
            }
            _clusterClient.Dispose();
            _disposed = true;
        }

        public Task Close()
        {
            return _clusterClient.Close();
        }

        public Task<Message> GetMail(Guid id)
        {
            throw new NotImplementedException();
        }

        public IPublicBoard GetPublicBoard()
        {
            throw new NotImplementedException();
        }

        public async Task<ISubjectOfInterest> GetSubjectOfInterest()
        {
            Guid? userID = await this.LoadUserId();
            return _clusterClient.GetGrain<ISubjectOfInterest>(userID.Value);
        }
    }
}
