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

        private IPersistentState<List<MailMessage>> _storageState;
        public EventMailbox([PersistentState("mailGrain")] IPersistentState<List<MailMessage>> storageState)
        {
            _storageState = storageState;
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
                await _storageState.ReadStateAsync();
                foreach (var mail in _storageState.State)
                {
                    await Stream.OnNextAsync(mail);
                }
            });
            return Task.CompletedTask;
        }
        public async Task MarkRead(Guid msgId)
        {
            await _storageState.ReadStateAsync();
            foreach (var mail in _storageState.State.Where(mail => mail.MsgId == msgId))
            {
                mail.ReadState = true;
            }
            await _storageState.WriteStateAsync();
        }
        public async Task<Guid> HasMail()
        {
            return await Task.FromResult(this.GetPrimaryKey());
        }

        public async Task<Guid> GetStreamId()
        {
            StreamId ??= Guid.NewGuid();
            return await Task.FromResult(StreamId.Value);
        }

        public async Task StartStream()
        {
            //how?
        }

        public async Task SendMail(MailMessage mail)
        {
            if (Stream == null)
            {
                Stream = this.GetStreamProvider(Constants.DefaultStream)
                    .GetStream<MailMessage>(StreamId.Value, Constants.DefaultNamespace);
            }
            await _storageState.ReadStateAsync();
            _storageState.State.Add(mail);
            await _storageState.WriteStateAsync();
            await Stream.OnNextAsync(mail);
        }

        public async Task DeleteMail(Guid msgId)
        {
            await _storageState.ReadStateAsync();
            foreach (var mail in _storageState.State)
            {
                _storageState.State.RemoveAll(e => e.MsgId == msgId);
            }
            await _storageState.WriteStateAsync();
        }
    }
}
