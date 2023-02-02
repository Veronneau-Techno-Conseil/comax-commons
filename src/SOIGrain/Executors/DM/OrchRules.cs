using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Comax.Commons.Orchestrator.SOIGrain.Executors.DM
{
    public static class OrchRules
    {
        private static string[] SupportedMessageTypes = new[] {
            MessageTypes.OrchestratorInstructions.MSG_TYPE_ACK_ALIVE,
            MessageTypes.OrchestratorInstructions.MSG_TYPE_SYNC_PORTFOLIO
        };

        
        public static void AddOrchDMRules(this SOIBroadcastRulesEngine sOIBroadcastRulesEngine, IStreamProvider streamProvider, IComaxGrainFactory comaxGrainFactory, ClaimsPrincipal cp)
        {
            sOIBroadcastRulesEngine.AddRule(new DirectMessageStreamExecutor(streamProvider, comaxGrainFactory),
                new RuleField<string>
                {
                    CheckAsync = async (v) =>
                    {
                        if (string.IsNullOrWhiteSpace(v) || !v.ToLower().StartsWith($"orch://"))
                            return false;
                        return await ValidationHelper.ValidateOrchId(comaxGrainFactory, v);
                    }
                },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith($"orch://") },
                new RuleField<string>
                {
                    Check = (v) =>
                    {
                        if (string.IsNullOrWhiteSpace(v) || !v.ToLower().StartsWith("com://"))
                            return false;
                        var u = MessageHelper.GetEntityId(v);
                        if (string.IsNullOrWhiteSpace(u))
                            return false;
                        return true;
                    }
                },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && SupportedMessageTypes.Contains(v.ToUpper()) },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals(MessageScopes.MSG_SCOPE_PRIVATE) }
            );
        }
    }
}
