using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;
using DispatchGrain.RuleEngine;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.Grains.DispatchGrain.RuleEngine
{
    public class DispatchRuleEngine : RuleEngine<Message>
    {
        public DispatchRuleEngine(IStreamProvider streamProvider, IComaxGrainFactory comaxGrainFactory)
        {
            AddRule(new DispatchEventStreamExecutor(streamProvider, comaxGrainFactory),
                new RuleField<string> { Ignore = true },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("usr://") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("usr://") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals(MessageTypes.Communicate.MSG_TYPE_DIRECT) },
                new RuleField<string> { Ignore = true }
            );

            AddRule(new DispatchEventStreamExecutor(streamProvider, comaxGrainFactory),
                new RuleField<string> { Ignore = true },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("orch://") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals(MessageTypes.OrchestratorInstructions.MSG_TYPE_ACK_ALIVE) },
                new RuleField<string> { Ignore = true }
            );

            AddRule(new TargetModifiedEventStreamExecutor(streamProvider, comaxGrainFactory, "portfolio", OrleansConstants.StreamNamespaces.PORTFOLIO_NOTIFS_NS, TargetModifiedEventStreamExecutor.StreamIdResolvingStrategy.GuidZero),
                new RuleField<string> { Ignore = true },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("orch://") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals(MessageTypes.OrchestratorInstructions.MSG_TYPE_SYNC_PORTFOLIO) },
                new RuleField<string> { Ignore = true }
            );
        }

        public override object[] ExtractValues(Message param)
        {
            return new object[] { param.From, param.FromOwner, param.To, param.Type, param.Scope };
        }
    }
}