using Comax.Commons.Orchestrator.Contracts.SOI;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SOIGrain
{
    [AuthorizeClaim]
    public class SubjectOfInterest : Grain, ISubjectOfInterest
    {
        public SubjectOfInterest()
        {
            
        }
        public async Task<OperationResult> Broadcast(Message message)
        {
            //TODO: Validate message from against authenticated user
            var comaxGrainFactory = new CommunAxiom.Commons.Orleans.GrainFactory(GrainFactory, this.GetStreamProvider);
            SOIBroadcastRulesEngine broadcastRulesEngine = new SOIBroadcastRulesEngine(this.GetUser(), this.GetStreamProvider(CommunAxiom.Commons.Orleans.OrleansConstants.Streams.ImplicitStream), comaxGrainFactory);
            var r = broadcastRulesEngine.Validate(message);
            if (r.IsError)
                return r;
            await broadcastRulesEngine.Process(message);
            return new OperationResult();
        }
    }
}
