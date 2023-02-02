using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Comax.Commons.Orchestrator.SOIGrain.Executors.DM
{
    public static class DMRules
    {
        public static void AddDMRules(this SOIBroadcastRulesEngine sOIBroadcastRulesEngine, IStreamProvider streamProvider, IComaxGrainFactory comaxGrainFactory, ClaimsPrincipal cp)
        {
            sOIBroadcastRulesEngine.AddRule(new DirectMessageStreamExecutor(streamProvider, comaxGrainFactory),
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
                        return await ValidationHelper.AuthorizeDirectUserMessage(comaxGrainFactory, u);
                    }
                },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals(MessageTypes.Communicate.MSG_TYPE_DIRECT) },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals(MessageScopes.MSG_SCOPE_PRIVATE) }
            );
        }
    }
}
