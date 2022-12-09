using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;

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
    }
}
