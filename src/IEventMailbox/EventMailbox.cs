using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunAxiom.Commons.Orleans;
using Orleans;
using Orleans.Streams;
using Orleans.Runtime;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Concurrency;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.CommonsShared.Contracts.Mail;
using Microsoft.Extensions.Logging;

namespace CommunAxiom.Commons.CommonsShared.EventMailboxGrain
{
    [Reentrant]
    [ImplicitStreamSubscription(EventMailboxConstants.MAILBOX_STREAM_INBOUND_NS)]
    [AuthorizeClaim(ClaimType = "https://orchestrator.communaxiom.org/mailbox")]
    public class EventMailbox : Grain, IEventMailbox
    {
        private readonly IPersistentState<MailboxState> _storageState;
        public EventMailboxBusiness _eventMailboxBusiness;
        private readonly ILogger _logger;
        public EventMailbox([PersistentState("mailGrain")] IPersistentState<MailboxState> storageState, ILogger<EventMailbox> logger)
        {
            _storageState = storageState;
            _logger = logger;

        }

        public async Task MarkRead(Guid msgId)
        {
            await _eventMailboxBusiness.MarkRead(msgId);
        }
        public async Task<bool> HasMail()
        {
            return await _eventMailboxBusiness.HasMail();
        }

        public async Task SendMail(MailMessage mail)
        {
            await _eventMailboxBusiness.DropMail(mail);
        }

        public async Task DeleteMail(Guid msgId)
        {
            await _eventMailboxBusiness.DeleteMail(msgId);
        }

        public async Task<Message> GetMessage(Guid msgId)
        {
            return await _eventMailboxBusiness.GetMessage(msgId);
        }

        public override async Task OnActivateAsync()
        {
            _eventMailboxBusiness = new EventMailboxBusiness(this.GetStreamProvider(Orleans.Constants.DefaultStream), new CommunAxiom.Commons.Orleans.GrainFactory(GrainFactory), _logger);
            _eventMailboxBusiness.Init(_storageState);

            var streamProvider = GetStreamProvider(Orleans.Constants.ImplicitStream);
            var key = this.GetPrimaryKey();
            var stream = streamProvider.GetStream<Message>(
                    key, EventMailboxConstants.MAILBOX_STREAM_INBOUND_NS);

            await stream.SubscribeAsync(async (msg, seqToken) =>
            {
                var mg = this.GrainFactory.GetGrain<IMail>(msg.Id);
                await mg.Save(msg);
                var mm = new MailMessage
                {
                    From = msg.From,
                    To = msg.To,
                    MsgId = msg.Id,
                    ReceivedDate = DateTime.UtcNow,
                    Subject = msg.Subject,
                    Type = msg.Type
                };
                await _eventMailboxBusiness.DropMail(mm);
            });

        }

        public async Task Subscribe(IMailboxObserver mailboxObserver)
        {
            await _eventMailboxBusiness.Subscribe(mailboxObserver);
        }

        public async Task Unsubscribe(IMailboxObserver mailboxObserver)
        {
            await _eventMailboxBusiness.Unsubscribe(mailboxObserver);
        }
    }
}
