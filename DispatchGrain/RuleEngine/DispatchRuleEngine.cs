using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.Grains.DispatchGrain.RuleEngine
{
    public class DispatchRuleEngine : RuleEngine<Message>
    {
        public DispatchRuleEngine(IStreamProvider streamProvider)
        {
            AddRule(new DispatchEventStreamExecutor(streamProvider),
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://local/data") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("usr://") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToLower().Equals("local") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("DATA_ACCESS_REQUEST") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("PARTNERS") }
            );   
        }

        public override object[] ExtractValues(Message param)
        {
            return new object[] { param.From, param.FromOwner, param.To, param.Type, param.Scope };
        }
    }
}