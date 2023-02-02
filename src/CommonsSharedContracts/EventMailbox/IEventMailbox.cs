using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;

namespace CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox
{
    public interface IEventMailbox:IGrainWithGuidKey
    {
        Task<bool> HasMail();
        Task MarkRead(Guid id);
        Task DeleteMail(Guid id);
        Task Subscribe(IMailboxObserver mailboxObserver);
        Task Unsubscribe(IMailboxObserver mailboxObserver);
        Task<Message> GetMessage(Guid msgId);
        Task SendMail(MailMessage mail);
        Task StreamMails(StreamSpec streamSpec);
    }
}
