using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neleus.DependencyInjection.Extensions;
using Shared.Tests.RulesEngine.Mock;
using Comax.Commons.Shared.RulesEngine;

namespace Shared.Tests.RulesEngine
{
    [TestFixture]
    public class RuleEngineSimpleTest
    {
        private IServiceProvider _serviceProvider;

        [OneTimeSetUp]
        public void TestSetup()
        {
            ServiceCollection servicesCollection = new ServiceCollection();
            servicesCollection.AddTransient<LocalExecutor>();
            servicesCollection.AddTransient<PublicExecutor>();
            servicesCollection.AddByName<IExecutor<MessageMock>>().Add<LocalExecutor>("LocalExecutor")
                                                                .Add<PublicExecutor>("PublicExecutor")
                                                                .Build();
            _serviceProvider = servicesCollection.BuildServiceProvider();
        }

        [Test]
        public async Task TestRulesEngine()
        {
            MessageRulesEngineMock messageRulesEngineMock = new MessageRulesEngineMock(_serviceProvider);

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
