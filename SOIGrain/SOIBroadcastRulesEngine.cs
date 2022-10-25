using Comax.Commons.Orchestrator.SOIGrain.Executors;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Comax.Commons.Orchestrator.SOIGrain
{

    public class SOIBroadcastRulesEngine : RuleEngine<Message>
    {
        private readonly IStreamProvider _streamProvider;
        private readonly IComaxGrainFactory _comaxGrainFactory;
        private readonly ClaimsPrincipal _claimsPrincipal;
        public SOIBroadcastRulesEngine(ClaimsPrincipal cp, IStreamProvider streamProvider, IComaxGrainFactory comaxGrainFactory)
        {
            _streamProvider = streamProvider;
            _comaxGrainFactory = comaxGrainFactory;
            _claimsPrincipal = cp;

            AddRule(new DirectMessageStreamExecutor(_streamProvider, _comaxGrainFactory),
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith($"usr://{cp.FindFirst("sub").Value}") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith($"usr://{cp.FindFirst("sub").Value}") },
                new RuleField<string>
                {
                    CheckAsync = async (v) =>
                        {
                            if (string.IsNullOrWhiteSpace(v) || !v.ToLower().StartsWith("usr://"))
                                return false;
                            var u = MessageHelper.GetUserId(v);
                            if (string.IsNullOrWhiteSpace(u))
                                return false;
                            return await ValidationHelper.AuthorizeDirectUserMessage(_comaxGrainFactory, u);
                        }
                },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals(MessageHelper.MSG_TYPE_DIRECT) },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals(MessageHelper.MSG_SCOPE_PRIVATE) }
            );

        }

        public override object[] ExtractValues(Message param)
        {
            return new object[] { param.From, param.FromOwner, param.To, param.Type, param.Scope };
        }

    }
}
