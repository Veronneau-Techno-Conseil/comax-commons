using Broadcast.Grian.Tests.Stream;
using CommunAxiom.Commons.Client.Grains.BroadcastGrain;
using CommunAxiom.Commons.Shared.RuleEngine;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Grian.Tests
{
    [TestFixture]
    public class LocalEventStreamExecutorTest
    {
        private readonly LocalEventStreamExecutor _localEventStreamExecutor;
        private readonly MessageStreamProvider _messageStreamProvider;
        
        public LocalEventStreamExecutorTest()
        {
            _messageStreamProvider = new MessageStreamProvider();
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
                Type = "NEW_DATA_VERSION",
                Scope = "PARTNERS"
            };

            await _localEventStreamExecutor.Execute(message);

            streamObserver.Message.Should().BeEquivalentTo(message);
        }
    }


}
