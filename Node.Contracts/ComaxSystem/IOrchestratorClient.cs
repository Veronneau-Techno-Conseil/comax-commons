using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Comax.Commons.Orchestrator.Contracts.UriRegistry;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.ComaxSystem
{
    public interface IOrchestratorClient: IDisposable
    {
        IMailbox GetMailbox(string id);
        IUriRegistry GetUriRegistry(string id);
        Task Close();
    }
}
