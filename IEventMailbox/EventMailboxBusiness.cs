using Comax.Commons.Orchestrator.MailGrain;
using CommunAxiom.Commons.Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Comax.Commons.Orchestrator.EventMailboxGrain
{
    public class EventMailboxBusiness
    {
        private EventMailboxRepo _EventMailboxRepo;

        public void Init(IPersistentState<List<MailMessage>> mailMessages)
        {
            _EventMailboxRepo = new EventMailboxRepo(mailMessages);
        }
        public async Task<List<MailMessage>> GetMailMessages()
        {
            return await _EventMailboxRepo.Fetch();
        }
        public async Task UpdateMailMessages(List<MailMessage> mailMessages)
        {
            await _EventMailboxRepo.Update(mailMessages);
        }
        public async Task MarkRead(Guid msgId)
        {
            var mailMessages = await _EventMailboxRepo.Fetch();
            foreach (var mail in mailMessages.Where(mail => mail.MsgId == msgId))
            {
                mail.ReadState = true;
            }
            await UpdateMailMessages(mailMessages);
        }
        public async Task DeleteMail(Guid msgId)
        {
            var mailMessages = await _EventMailboxRepo.Fetch();
            foreach (var mail in mailMessages)
            {
                mailMessages.RemoveAll(e => e.MsgId == msgId);
            }
            await UpdateMailMessages(mailMessages);
        }
        public async Task ResumeMessageStream(IAsyncStream<MailMessage> stream)
        {
            var mailMessages = await GetMailMessages();
            foreach (var mail in mailMessages)
            {
                await stream.OnNextAsync(mail);
            }
        }
        public async Task SendMail(IAsyncStream<MailMessage> stream, MailMessage mail)
        {
            var mailMessages = await GetMailMessages();
            mailMessages.Add(mail);
            await UpdateMailMessages(mailMessages);
            await stream.OnNextAsync(mail);
        }

    }
}
