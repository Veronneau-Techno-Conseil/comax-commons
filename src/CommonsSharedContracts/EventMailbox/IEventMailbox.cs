using System;
using System.Threading.Tasks;
using Orleans;

namespace CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox
{
    public interface IEventMailbox:IGrainWithGuidKey
    {
        Task<bool> HasMail();
        Task<Guid> GetStreamId();
        Task MarkRead(Guid id);
        Task DeleteMail(Guid id);
        Task StartStream();
        Task SendMail(MailMessage mail);

    }
}
