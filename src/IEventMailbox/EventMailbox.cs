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
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Runtime.InteropServices;

namespace CommunAxiom.Commons.CommonsShared.EventMailboxGrain
{
    [Reentrant]
    [ImplicitStreamSubscription(OrleansConstants.StreamNamespaces.MAILBOX_STREAM_INBOUND_NS)]
    //[AuthorizeClaim(ClaimType = "https://orchestrator.communaxiom.org/mailbox")]
    public class EventMailbox : Grain, IEventMailbox
    {
        private readonly IPersistentState<MailboxState> _storageState;
        public EventMailboxBusiness _eventMailboxBusiness;
        private ILogger _logger;
        private IDisposable? _cleanupSvc;
        public EventMailbox([PersistentState("mailGrain")] IPersistentState<MailboxState> storageState)
        {
            _storageState = storageState;

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
            _logger = ServiceProvider.GetService<ILogger<EventMailbox>>();
            _eventMailboxBusiness = new EventMailboxBusiness(new GrainFactory(GrainFactory, this.GetStreamProvider), _logger);
            _eventMailboxBusiness.Init(_storageState);

            var streamProvider = GetStreamProvider(OrleansConstants.Streams.ImplicitStream);
            var key = this.GetPrimaryKey();
            var stream = streamProvider.GetStream<Message>(
                    key, OrleansConstants.StreamNamespaces.MAILBOX_STREAM_INBOUND_NS);

            await stream.SubscribeAsync(async (msg, seqToken) =>
            {
                try
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
                }
                catch(Exception ex)
                {
                    throw;
                }
            });

        }

        private void SetCleanupSvc()
        {
            _cleanupSvc = RegisterTimer(o => DoCleanup(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        private async Task DoCleanup()
        {
            var cnt = await _eventMailboxBusiness.Cleanup();
            if(cnt == 0)
            {
                _cleanupSvc?.Dispose();
                _cleanupSvc = null;
            }
        }

        public async Task Subscribe(IMailboxObserver mailboxObserver)
        {
            SetCleanupSvc();
            await _eventMailboxBusiness.Subscribe(mailboxObserver);
        }

        public async Task Unsubscribe(IMailboxObserver mailboxObserver)
        {
            SetCleanupSvc();
            await _eventMailboxBusiness.Unsubscribe(mailboxObserver);
        }

        public async Task StreamMails(StreamSpec streamSpec)
        {
            SetCleanupSvc();
            await _eventMailboxBusiness.StreamMails(streamSpec);
        }
    }
}
