using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared;
using Shared.Tests.RuleEngine.Mock;

namespace Shared.Tests.RulesEngine.Mock
{
    public class MessageRulesEngineMock : RuleEngine<MessageMock>
    {
        public MessageRulesEngineMock()
        {
            this.AddRule(new PublicExecutor(),
            /* from: call from commons */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://") },
            /* to: destination wildcards for all commons clients */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToLower().Equals("com://*") },
            /* type: what event is sent */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("NEW_DATA_VERSION") },
            /* scope: who can receive */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("PARTNERS") }
            );
            this.AddRule(new LocalExecutor(),
            /* from: call from commons */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://") },
            /* to: ignore destination on local scope */ new RuleField<string> { Ignore = true },
            /* type: what event is sent */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("NEW_DATA_VERSION") },
            /* scope: who can receive */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && (v.ToUpper().Equals("LOCAL") || v.ToUpper().Equals("PARTNERS")) }
            );

            this.AddRule(new LocalExecutor(),
            /* from: call from commons */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://") },
            /* to: ignore destination on local scope */ new RuleField<string> { Ignore = true },
            /* type: what event is sent */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("INGESTION_COMPLETE") },
            /* scope: who can receive */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("LOCAL") }
            );

            this.AddRule(new PublicExecutor(),
            /* from: call from commons */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.StartsWith("com://") },
            /* to: destination wildcards for all commons clients */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToLower().Equals("com://*") },
            /* type: what event is sent */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("NEW_PORTFOLIO_ELEMENT") },
            /* scope: who can receive */ new RuleField<string> { Check = (v) => !string.IsNullOrWhiteSpace(v) && v.ToUpper().Equals("PARTNERS") }
            );
        }
        protected override OperationResult Validate(params object[] values)
        {
            return base.Validate(values);
        }

        public override object[] ExtractValues(MessageMock param)
        {
            return new object[] { param.From, param.To, param.Type, param.Scope };
        }
    }
}
