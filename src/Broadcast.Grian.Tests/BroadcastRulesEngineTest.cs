using CommunAxiom.Commons.Client.Grains.BroadcastGrain;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using IStreamProvider = Orleans.Streams.IStreamProvider;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;

namespace Broadcast.Grian.Tests
{
    [TestFixture]
    public class BroadcastRulesEngineTest
    {
        private BroadcastRulesEngine _broadcastRulesEngine;
        private MockRepository _mockRepository;
        private Mock<IStreamProvider> _streamProvider;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _streamProvider = new Mock<IStreamProvider>();

            _broadcastRulesEngine = new BroadcastRulesEngine(_streamProvider.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockRepository.Verify();
        }


        [Test]
        public void ShouldMapExtractValues()
        {
            var message = new Message
            {
                From = "com://local",
                FromOwner = "ust://{usrid}",
                To = "com://*",
                Type = MessageTypes.CommonsAgentEvents.MSG_TYPE_NEW_DATA,
                Scope = MessageScopes.MSG_SCOPE_PARTNERS
            };

            var result = _broadcastRulesEngine.ExtractValues(message);


            result.Should().NotBeEmpty();
            result[0].Should().Be("com://local");
            result[1].Should().Be("ust://{usrid}");
            result[2].Should().Be("com://*");
            result[3].Should().Be(MessageTypes.CommonsAgentEvents.MSG_TYPE_NEW_DATA);
            result[4].Should().Be(MessageScopes.MSG_SCOPE_PARTNERS);
        }

        [Test]
        public void WhenRuleEngineSetupShouldReturnRouleTable()
        {
            _broadcastRulesEngine.RulesTable.Should().HaveCount(3);
        }

        [Test]
        public void WhenRuleEngineSetupShouldSetFieldCount()
        {
            _broadcastRulesEngine.FieldCount.Should().Be(5);
        }

        [Test]
        public void WhenRuleEngineSetupShouldSetExecutors()
        {
            var rule1 = _broadcastRulesEngine.RulesTable[0];
            var rule2 = _broadcastRulesEngine.RulesTable[1];
            var rule3 = _broadcastRulesEngine.RulesTable[2];

            rule1.Executor.Should().BeOfType<LocalEventStreamExecutor>();
            rule2.Executor.Should().BeOfType<LocalEventStreamExecutor>();
            rule3.Executor.Should().BeOfType<OrchestratorEventStreamExecutor>();
        }

        [Test]
        public void WhenRuleEngineSetupShouldSetConfigFields()
        {
            var rule1 = _broadcastRulesEngine.RulesTable[0];
            var rule2 = _broadcastRulesEngine.RulesTable[1];
            var rule3 = _broadcastRulesEngine.RulesTable[2];

            rule1.ConfigFields.Should().HaveCount(5);
            rule2.ConfigFields.Should().HaveCount(5);
            rule3.ConfigFields.Should().HaveCount(5);
        }

        [Test]
        public void WhenValidateTrueThenIsErrorShouldReturnFalse()
        {
            var message = new Message
            {
                From = "com://local",
                FromOwner = "ust://{usrid}",
                To = "com://*",
                Type = MessageTypes.CommonsAgentEvents.MSG_TYPE_NEW_DATA,
                Scope = MessageScopes.MSG_SCOPE_PARTNERS
            };

            var result = _broadcastRulesEngine.Validate(message);

            result.Should().NotBeNull();

            result.Detail.Should().BeNull();
            result.Error.Should().BeNull();
            result.IsError.Should().BeFalse();
        }

    }
}