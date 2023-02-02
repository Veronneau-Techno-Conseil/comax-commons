using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;

namespace CommunAxiom.Commons.CommonsShared.Contracts.Mail
{
    public interface IMail : IGrainWithGuidKey
    {
        Task<bool> Exists();
        Task<Message> GetMessage();
        Task Save(Message message);
        Task Delete();
    }
}
