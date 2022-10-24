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
    public class SubjectOfInterest : Grain, ISubjectOfInterest
    {
        private readonly IComaxGrainFactory _comaxGrainFactory;
        public SubjectOfInterest()
        {
            _comaxGrainFactory = new CommunAxiom.Commons.Orleans.GrainFactory(GrainFactory);
        }
        public async Task<OperationResult> Broadcast(Message message)
        {
            SOIBroadcastRulesEngine broadcastRulesEngine = new SOIBroadcastRulesEngine(this.GetUser(), this.GetStreamProvider(Constants.DefaultStream), _comaxGrainFactory);
            var r = broadcastRulesEngine.Validate(message);
            if (r.IsError)
                return r;
            _ = broadcastRulesEngine.Process(message);
            return new OperationResult();
        }
    }
}
