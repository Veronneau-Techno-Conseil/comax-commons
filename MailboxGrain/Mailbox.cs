using Comax.Commons.Orchestrator.Contracts.Mailbox;
using CommunAxiom.Commons.Orleans.Security;
using Orleans;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MailboxGrain
{
    
    [AuthorizeClaim(ClaimType = "https://orchestrator.communaxiom.org/mailbox")]
    public class Mailbox : Grain, IMailbox
    {
        public Task<bool> HasMail()
        {
            return Task.FromResult(false);
        }
    }
}