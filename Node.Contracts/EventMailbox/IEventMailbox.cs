using System;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.MailGrain;
using Orleans;

namespace Comax.Commons.Orchestrator.Contracts.EventMailbox
{
    public interface IEventMailbox:IGrainWithGuidKey
    {
        Task<Guid> HasMail();
        Task<Guid> GetStreamId();
        Task MarkRead(Guid id);
        Task DeleteMail(Guid id);
        Task StartStream();
        Task SendMail(MailMessage mail);

    }
}
