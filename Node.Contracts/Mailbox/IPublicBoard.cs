using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;

namespace Comax.Commons.Orchestrator.Contracts.Mailbox
{
    [AuthorizeClaim()]
    public interface IPublicBoard : IGrainWithGuidKey
    {
        Task Broadcast(Message message);
        Task Subscribe(IPublicBoardObserver publicBoardObserver, Guid latestMsg);
        Task Unsubscribe(IPublicBoardObserver publicBoardObserver);
    }
}
