using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.DependencyInjection;
using Neleus.DependencyInjection.Extensions;
using NUnit.Framework;
using Shared.Tests.RuleEngine.Mock;
using Shared.Tests.RulesEngine.Mock;

namespace Shared.Tests.RulesEngine
{
    [TestFixture]
    public class RuleEngineSimpleTest
    {
        [Test]
        public async Task TestRulesEngine()
        {
            MessageRulesEngineMock messageRulesEngineMock = new MessageRulesEngineMock();

            var message = new MessageMock
            {
                From = "com://local",
                To = "com://*",
                Type = "NEW_DATA_VERSION",
                Scope = "PARTNERS"
            };

            var res = messageRulesEngineMock.Validate(message);
            Assert.IsTrue(!res.IsError);

            await messageRulesEngineMock.Process(message);

            Assert.AreSame(message, ExecutorTargets.PublicTarget);
            Assert.AreSame(message, ExecutorTargets.LocalTarget);

            ExecutorTargets.Flush();

            message = new MessageMock
            {
                From = "com://local",
                To = "Never gonna give you up. Never gonna let you down. Never gonna run around and desert you",
                Type = "NEW_DATA_VERSION",
                Scope = "LOCAL"
            };
            
            await messageRulesEngineMock.Process(message);

            Assert.IsNull(ExecutorTargets.PublicTarget);
            Assert.AreSame(message, ExecutorTargets.LocalTarget);
        }
    }
}
