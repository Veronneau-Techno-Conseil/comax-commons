using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using Comax.Commons.Orchestrator.MailGrain;
using CommunAxiom.Commons.Orleans;
using Orleans;
using Orleans.Streams;
using Orleans.Runtime;

namespace Comax.Commons.Orchestrator.EventMailboxGrain
{
    public class EventMailbox: Grain,IEventMailbox
    {
        public Guid? StreamId { get; set; }
        public IAsyncStream<MailMessage> Stream { get; set; }
        public readonly EventMailboxBusiness _eventMailboxBusiness;
        public EventMailbox(EventMailboxBusiness eventMailboxBusiness,[PersistentState("mailGrain")] IPersistentState<List<MailMessage>> storageState)
        {
            _eventMailboxBusiness = eventMailboxBusiness;
            _eventMailboxBusiness.Init(storageState);
        }


        public Task ResumeMessageStream()
        {
            if (Stream == null)
            {
                Stream = this.GetStreamProvider(Constants.DefaultStream)
                    .GetStream<MailMessage>(StreamId.Value, Constants.DefaultNamespace);
            }

            _= Task.Run(async () =>
            {
                var mailMessages = await _eventMailboxBusiness.GetMailMessages();
                foreach (var mail in mailMessages)
                {
                    await Stream.OnNextAsync(mail);
                }
            });
            return Task.CompletedTask;
        }
        public async Task MarkRead(Guid msgId)
        {
            await _eventMailboxBusiness.MarkRead(msgId);
        }
        public async Task<Guid> HasMail()
        {
            return await Task.FromResult(StreamId.Value);
        }

        public async Task<Guid> GetStreamId()
        {
            StreamId ??= Guid.NewGuid();
            return await Task.FromResult(StreamId.Value);
        }

        public async Task StartStream()
        {
            //read storage and push to stream
            await ResumeMessageStream();
        }

        public async Task SendMail(MailMessage mail)
        {
            if (Stream == null)
            {
                Stream = this.GetStreamProvider(Constants.DefaultStream)
                    .GetStream<MailMessage>(StreamId.Value, Constants.DefaultNamespace);
            }
            var mailMessages = await _eventMailboxBusiness.GetMailMessages();
            mailMessages.Add(mail);
            await _eventMailboxBusiness.UpdateMailMessages(mailMessages);
            await Stream.OnNextAsync(mail);
        }

        public async Task DeleteMail(Guid msgId)
        {
            await _eventMailboxBusiness.DeleteMail(msgId);
        }
    }
}
