using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using Orleans.Streams;

namespace CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox
{
    public interface IEventMailboxClient
    {
        Task<bool> HasMail();
        Task MarkRead(Guid id);
        Task DeleteMail(Guid id);
        Task<IAsyncDisposable> Subscribe(IMailboxObserver mailboxObserver);
        Task SendMail(MailMessage mail);
        Task<Message> GetMessage(Guid msgId);
        Task<(AsyncEnumerableStream<MailMessage>, StreamSubscriptionHandle<MailMessage>)> GetStream();
        Task<StreamSubscriptionHandle<MailMessage>> GetStream(Func<MailMessage, StreamSequenceToken, Task> fn, Func<Exception, Task> funcError, Func<Task> onCompleted);
    }
}
