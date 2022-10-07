using System.Collections.Generic;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.MailGrain;
using Orleans.Runtime;

namespace Comax.Commons.Orchestrator.EventMailboxGrain
{
    public class EventMailboxRepo
    {
        private readonly IPersistentState<List<MailMessage>> _EventMailboxRepo;

        public EventMailboxRepo(IPersistentState<List<MailMessage>> mailMessageRepo)
        {
            this._EventMailboxRepo = mailMessageRepo;
        }

        public async Task<List<MailMessage>> Fetch()
        {
            await _EventMailboxRepo.ReadStateAsync();
            return _EventMailboxRepo.State;
        }

        public async Task Update(List<MailMessage> mailMessage)
        {
            _EventMailboxRepo.State=mailMessage;
            await _EventMailboxRepo.WriteStateAsync();
        }
    }
}
