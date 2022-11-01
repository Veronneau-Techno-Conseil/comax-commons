using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using Orleans.Runtime;

namespace Comax.Commons.Orchestrator.EventMailboxGrain
{
    public class EventMailboxRepo
    {
        private readonly IPersistentState<MailboxState> _eventMailboxRepo;

        public EventMailboxRepo(IPersistentState<MailboxState> mailMessageRepo)
        {
            this._eventMailboxRepo = mailMessageRepo;
        }

        public async Task<MailMessage> Find(Guid msgId)
        {
            await _eventMailboxRepo.ReadStateAsync();
            return _eventMailboxRepo.State.MailMessages.FirstOrDefault(x => x.MsgId == msgId);
        }
        
        public async Task Remove(Guid msgId)
        {
            await _eventMailboxRepo.ReadStateAsync();
            _eventMailboxRepo.State.MailMessages.RemoveAll(x => x.MsgId == msgId);
            await _eventMailboxRepo.WriteStateAsync();
        }

        public async Task<IEnumerable<MailMessage>> Fetch()
        {
            await _eventMailboxRepo.ReadStateAsync();
            return _eventMailboxRepo.State.MailMessages.ToArray();
        }

        public async Task Add(MailMessage mailMessage)
        {
            await _eventMailboxRepo.ReadStateAsync();
            if (_eventMailboxRepo.State == null)
                _eventMailboxRepo.State = new MailboxState();
            _eventMailboxRepo.State.MailMessages.Add(mailMessage);
            await _eventMailboxRepo.WriteStateAsync();
        }

        public async Task Save()
        {
            await _eventMailboxRepo.WriteStateAsync();
        }

        public async Task<bool> HasMail()
        {
            var data = await Fetch();
            return data != null && data.Count() > 0;
        }
    }
}
