
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox
{
    public class EventMailboxClient : IEventMailboxClient
    {
        private readonly IClusterClient _clusterClient;
        private readonly IEventMailbox _eventMailbox;
        private readonly ILogger _logger;
        public EventMailboxClient(IClusterClient clusterClient, IEventMailbox eventMailbox, ILogger logger)
        {
            _clusterClient = clusterClient;
            _eventMailbox = eventMailbox;
            _logger = logger;
        }

        public Task DeleteMail(Guid id)
        {
            return _eventMailbox.DeleteMail(id);
        }

        public Task<Message> GetMessage(Guid msgId)
        {
            return _eventMailbox.GetMessage(msgId);
        }

        public Task<bool> HasMail()
        {
            return _eventMailbox.HasMail();
        }

        public Task MarkRead(Guid id)
        {
            return _eventMailbox.MarkRead(id);
        }

        public Task SendMail(MailMessage mail)
        {
            return _eventMailbox.SendMail(mail);
        }

        public async Task<IAsyncDisposable> Subscribe(IMailboxObserver mailboxObserver)
        {
            var obsRef = await this._clusterClient.CreateObjectReference<IMailboxObserver>(mailboxObserver);
            Orleans.GrainObserverSubscription<IMailboxObserver> grainObserverSubscription =
                new Orleans.GrainObserverSubscription<IMailboxObserver>(mailboxObserver,
                                                                         obsRef,
                                                                         (o) => _eventMailbox.Subscribe(o),
                                                                         (o) => _eventMailbox.Unsubscribe(o),
                                                                         _logger);
            return grainObserverSubscription;
        }

        public async Task<(AsyncEnumerableStream<MailMessage>, StreamSubscriptionHandle<MailMessage>)> GetStream()
        {
            Guid guid= Guid.NewGuid();
            var res = new AsyncEnumerableStream<MailMessage>();
            var sp = this._clusterClient.GetStreamProvider(OrleansConstants.Streams.DefaultStream);
            var strm = sp.GetStream<MailMessage>(guid, Orleans.OrleansConstants.StreamNamespaces.DefaultNamespace);
            var handle = await strm.SubscribeAsync(res);

            await this._eventMailbox.StreamMails(new StreamSpec { Id=guid, StreamProvider = OrleansConstants.Streams.DefaultStream, Namespace= OrleansConstants.StreamNamespaces.DefaultNamespace });

            return (res, handle);
        }

        public async Task<StreamSubscriptionHandle<MailMessage>> GetStream(Func<MailMessage, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted)
        {
            Guid guid = Guid.NewGuid();
            var sp = this._clusterClient.GetStreamProvider(Orleans.OrleansConstants.Streams.DefaultStream);
            var strm = sp.GetStream<MailMessage>(guid, Orleans.OrleansConstants.StreamNamespaces.DefaultNamespace);
            var handle = await strm.SubscribeAsync(fn, funcError, onCompleted);
            await this._eventMailbox.StreamMails(new StreamSpec { Id = guid, Namespace = OrleansConstants.StreamNamespaces.DefaultNamespace, StreamProvider = OrleansConstants.Streams.ImplicitStream });

            return handle;
        }
    }

    public class EventMailboxClusterClient : IEventMailboxClient
    {
        private readonly IComaxGrainFactory _grainFactory;
        private readonly IEventMailbox _eventMailbox;
        private readonly ILogger _logger;
        public EventMailboxClusterClient(IComaxGrainFactory grainFactory, IEventMailbox eventMailbox, ILogger logger)
        {
            _grainFactory = grainFactory;
            _eventMailbox = eventMailbox;
            _logger = logger;
        }

        public Task DeleteMail(Guid id)
        {
            return _eventMailbox.DeleteMail(id);
        }

        public Task<Message> GetMessage(Guid msgId)
        {
            return _eventMailbox.GetMessage(msgId);
        }

        public Task<bool> HasMail()
        {
            return _eventMailbox.HasMail();
        }

        public Task MarkRead(Guid id)
        {
            return _eventMailbox.MarkRead(id);
        }

        public Task SendMail(MailMessage mail)
        {
            return _eventMailbox.SendMail(mail);
        }

        public async Task<IAsyncDisposable> Subscribe(IMailboxObserver mailboxObserver)
        {
            var obsRef = await this._grainFactory.CreateObjectReference<IMailboxObserver>(mailboxObserver);
            Orleans.GrainObserverSubscription<IMailboxObserver> grainObserverSubscription =
                new Orleans.GrainObserverSubscription<IMailboxObserver>(mailboxObserver,
                                                                         obsRef,
                                                                         (o) => _eventMailbox.Subscribe(o),
                                                                         (o) => _eventMailbox.Unsubscribe(o),
                                                                         _logger);
            return grainObserverSubscription;
        }

        public async Task<(AsyncEnumerableStream<MailMessage>, StreamSubscriptionHandle<MailMessage>)> GetStream()
        {
            Guid guid = Guid.NewGuid();
            var res = new AsyncEnumerableStream<MailMessage>();
            var sp = this._grainFactory.GetStreamProvider(Orleans.OrleansConstants.Streams.ImplicitStream);
            var strm = sp.GetStream<MailMessage>(guid, Orleans.OrleansConstants.StreamNamespaces.DefaultNamespace);
            var handle = await strm.SubscribeAsync(res);

            await this._eventMailbox.StreamMails(new StreamSpec { Id = guid, Namespace = OrleansConstants.StreamNamespaces.DefaultNamespace, StreamProvider = OrleansConstants.Streams.ImplicitStream });

            return (res, handle);
        }

        public async Task<StreamSubscriptionHandle<MailMessage>> GetStream(Func<MailMessage, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted)
        {
            Guid guid = Guid.NewGuid();
            var sp = this._grainFactory.GetStreamProvider(Orleans.OrleansConstants.Streams.ImplicitStream);
            var strm = sp.GetStream<MailMessage>(guid, Orleans.OrleansConstants.StreamNamespaces.DefaultNamespace);
            var handle = await strm.SubscribeAsync(fn, funcError, onCompleted);
            await this._eventMailbox.StreamMails(new StreamSpec { Id = guid, Namespace = OrleansConstants.StreamNamespaces.DefaultNamespace, StreamProvider = OrleansConstants.Streams.ImplicitStream });

            return handle;
        }
    }
}
