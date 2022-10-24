using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Comax.Commons.Orchestrator.Contracts.UriRegistry;
using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.Central;

namespace Comax.Commons.Orchestrator.Contracts.ComaxSystem
{
    public interface IOrchestratorClient: IDisposable
    {
        IMailbox GetMailbox(string id);
        IUriRegistry GetUriRegistry(string id);
        IEventMailbox GetEventMailbox(Guid id);
        ICentral GetCentral();
        Task Close();
    }
}
