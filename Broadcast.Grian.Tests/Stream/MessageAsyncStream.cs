using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;

namespace Broadcast.Grian.Tests.Stream
{
    public class MessageAsyncStream : IAsyncStream<Message>
    {
        private IAsyncObserver<Message> asyncObserver;

        public bool IsRewindable => false;

        public string ProviderName => string.Empty;

        public Guid Guid => Guid.Empty;

        public string Namespace => string.Empty;

        public int CompareTo(IAsyncStream<Message>? other) => 0;

        public bool Equals(IAsyncStream<Message>? other) => true;

        public Task<IList<StreamSubscriptionHandle<Message>>> GetAllSubscriptionHandles() =>
            Task.FromResult<IList<StreamSubscriptionHandle<Message>>>(null);

        public Task OnCompletedAsync() => asyncObserver.OnCompletedAsync();

        public Task OnErrorAsync(Exception ex) => asyncObserver.OnErrorAsync(ex);

        public Task OnNextAsync(Message item, StreamSequenceToken token = null) => asyncObserver.OnNextAsync(item, token);

        public Task OnNextBatchAsync(IEnumerable<Message> batch, StreamSequenceToken token = null) => Task.CompletedTask;

        public Task<StreamSubscriptionHandle<Message>> SubscribeAsync(IAsyncObserver<Message> observer)
        {
            asyncObserver = observer;
            return Task.FromResult<StreamSubscriptionHandle<Message>>(null);
        }

        public Task<StreamSubscriptionHandle<Message>> SubscribeAsync(IAsyncObserver<Message> observer,
            StreamSequenceToken token, StreamFilterPredicate filterFunc = null, object filterData = null)
            => Task.FromResult<StreamSubscriptionHandle<Message>>(null);

        public Task<StreamSubscriptionHandle<Message>> SubscribeAsync(IAsyncBatchObserver<Message> observer)
            => Task.FromResult<StreamSubscriptionHandle<Message>>(null);

        public Task<StreamSubscriptionHandle<Message>> SubscribeAsync(IAsyncBatchObserver<Message> observer,
            StreamSequenceToken token)
            => Task.FromResult<StreamSubscriptionHandle<Message>>(null);

    }
}
