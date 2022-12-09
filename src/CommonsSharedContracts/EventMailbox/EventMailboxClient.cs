
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox
{
    public class EventMailboxClient: IEventMailboxClient
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

    }
}
