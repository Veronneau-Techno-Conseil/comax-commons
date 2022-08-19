using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;
using System;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class BroadcastRulesEngine : RuleEngine<Message>
    {
        public BroadcastRulesEngine(IServiceProvider serviceProvider, IStreamProvider streamProvider) : base(serviceProvider)
        {

            AddRule(new LocalEventStreamExecutor(streamProvider),
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://local/data/{dsid}") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("usr://{dsid}") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToLower().Equals("local") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("INGESTION_STARTED") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("PARTNERS") }
            );

            AddRule(new LocalEventStreamExecutor(streamProvider),
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://local/data/{dsid}") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("usr://{dsid}") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToLower().Equals("local") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("INGESTION_END") },
                new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("PARTNERS") }
           );
            
           AddRule(new OrchestratorEventStreamExecutor(streamProvider),
               new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://local/data/{dsid}") },
               new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("usr://{dsid}") },
               new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToLower().Equals("com://*") },
               new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("NEW_DATA_VERSION") },
               new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("PARTNERS") }
           );

        }

        public override object[] ExtractValues(Message param)
        {
            return new object[] { param.From, param.FromOwner, param.To, param.Type, param.Scope };
        }
    }
}
