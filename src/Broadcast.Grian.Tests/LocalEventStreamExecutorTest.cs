using Broadcast.Grian.Tests.Stream;
using CommunAxiom.Commons.Client.Grains.BroadcastGrain;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;
using FluentAssertions;
using GrainTests.Shared;
using NUnit.Framework;

namespace Broadcast.Grian.Tests
{
    [TestFixture]
    public class LocalEventStreamExecutorTest
    {
        private readonly LocalEventStreamExecutor _localEventStreamExecutor;
        private readonly AsyncStreamProvider<Message> _messageStreamProvider;
        
        public LocalEventStreamExecutorTest()
        {
            _messageStreamProvider = new AsyncStreamProvider<Message>();
            _localEventStreamExecutor = new LocalEventStreamExecutor(_messageStreamProvider);
        }

        [Test]
        public async Task LocalEventStreamExecutor()
        {
            var stream = _messageStreamProvider.GetStream<Message>(Guid.Empty, null);

            var streamObserver = new MessageStreamObserver();
            await stream.SubscribeAsync(streamObserver);

            var message = new Message
            {
                From = "com://local",
                FromOwner = "ust://{usrid}",
                To = "com://*",
                Type = MessageTypes.CommonsAgentEvents.MSG_TYPE_NEW_DATA,
                Scope = MessageScopes.MSG_SCOPE_PARTNERS
            };

            await _localEventStreamExecutor.Execute(message);

            streamObserver.Message.Should().BeEquivalentTo(message);
        }
    }


}
