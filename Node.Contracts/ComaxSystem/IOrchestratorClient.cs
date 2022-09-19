using Comax.Commons.Orchestrator.Contracts.Mailbox;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.ComaxSystem
{
    public interface IOrchestratorClient: IDisposable
    {
        IMailbox GetMailbox(string id);
        
        Task Close();
    }
}
