using CommunAxiom.Commons.Shared.RuleEngine;
using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.Mailbox
{
    public interface IMailItem : IGrainWithGuidKey
    {
        Task SaveData(Message o);
        Task<Message> GetData();
    }
}
