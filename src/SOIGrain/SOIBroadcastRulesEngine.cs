using Comax.Commons.Orchestrator.SOIGrain.Executors;
using Comax.Commons.Orchestrator.SOIGrain.Executors.DM;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Comax.Commons.Orchestrator.SOIGrain
{

    public class SOIBroadcastRulesEngine : RuleEngine<Message>, IUserContextAccessor
    {
        private readonly IStreamProvider _streamProvider;
        private readonly IComaxGrainFactory _comaxGrainFactory;
        private readonly ClaimsPrincipal _claimsPrincipal;
        public SOIBroadcastRulesEngine(ClaimsPrincipal cp, IStreamProvider streamProvider, IComaxGrainFactory comaxGrainFactory)
        {
            _streamProvider = streamProvider;
            _comaxGrainFactory = comaxGrainFactory;
            _claimsPrincipal = this.GetUser();

            this.AddDMRules(streamProvider, comaxGrainFactory, _claimsPrincipal);
            this.AddOrchDMRules(streamProvider, comaxGrainFactory, _claimsPrincipal);
        }

        public override object[] ExtractValues(Message param)
        {
            return new object[] { param.From, param.FromOwner, param.To, param.Type, param.Scope };
        }

    }
}
