using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Comax.Commons.Orchestrator.Contracts.UriRegistry;
using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.MailGrain;

namespace Comax.Commons.Orchestrator.Contracts.ComaxSystem
{
    public interface IOrchestratorClient: IDisposable
    {
        IMailbox GetMailbox(string id);
        IUriRegistry GetUriRegistry(string id);
        IEventMailbox GetEventMailbox(Guid id);
        Task<StreamSubscriptionHandle<MailMessage>> SubscribeEventMailboxStream(Guid id, Func<MailMessage, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted);
        Task Close();
    }
}
