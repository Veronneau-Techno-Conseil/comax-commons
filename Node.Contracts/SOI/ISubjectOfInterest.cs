using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.SOI
{
    public interface ISubjectOfInterest: IGrainWithGuidKey
    {
        Task<OperationResult> Broadcast(Message message);
    }
}
