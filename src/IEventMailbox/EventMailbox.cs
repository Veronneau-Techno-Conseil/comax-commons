﻿using System;
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

namespace CommunAxiom.Commons.CommonsShared.EventMailboxGrain
{
    [Reentrant]
    [ImplicitStreamSubscription(EventMailboxConstants.MAILBOX_STREAM_INBOUND_NS)]
    [AuthorizeClaim(ClaimType = "https://orchestrator.communaxiom.org/mailbox")]
    public class EventMailbox: Grain, IEventMailbox
    {
        private readonly IPersistentState<MailboxState> _storageState;
        public EventMailboxBusiness _eventMailboxBusiness;
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

        public async Task<Guid> GetStreamId()
        {
            return await _eventMailboxBusiness.GetStreamId();
        }

        public async Task StartStream()
        {
            //read storage and push to stream
            await _eventMailboxBusiness.ResumeMessageStream();

        }

        public async Task SendMail(MailMessage mail)
        {
            await _eventMailboxBusiness.DropMail(mail);
        }

        public async Task DeleteMail(Guid msgId)
        {
            await _eventMailboxBusiness.DeleteMail(msgId);
        }

        public override async Task OnActivateAsync()
        {
            _eventMailboxBusiness = new EventMailboxBusiness(this.GetStreamProvider(Constants.DefaultStream), new CommunAxiom.Commons.Orleans.GrainFactory(GrainFactory));
            _eventMailboxBusiness.Init(_storageState);

            var streamProvider = GetStreamProvider(Constants.ImplicitStream);
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
                    MsgId = msg.Id,
                    ReceivedDate = DateTime.UtcNow,
                    Subject = msg.Subject,
                    Type = msg.Type
                };
                await _eventMailboxBusiness.DropMail(mm);
            });
            
        }
    }
}