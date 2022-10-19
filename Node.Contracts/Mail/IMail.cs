using System;
using System.Threading.Tasks;
using Orleans;

namespace Comax.Commons.Orchestrator.Contracts.Mail
{
    public interface IMail : IGrainWithGuidKey
    {
        Task ResumeMessageStream();
        Task<bool> HasMail();
    }
}
