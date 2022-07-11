using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.Mailbox
{
    public interface IMailbox : IGrainWithStringKey
    {
        Task<bool> HasMail();
    }
}
