using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Orleans;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MailboxGrain
{
    public class Mailbox : Grain, IMailbox
    {
        public Task<bool> HasMail()
        {
            return Task.FromResult(false);
        }
    }
}