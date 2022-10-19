using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainTests.Shared
{
    public class AsyncStreamMock<T> : IAsyncStream<T>
    {
        private IAsyncObserver<T> asyncObserver;

        public bool IsRewindable => false;

        public string ProviderName => string.Empty;

        public Guid Guid => Guid.Empty;

        public string Namespace => string.Empty;

        public int CompareTo(IAsyncStream<T>? other) => 0;

        public bool Equals(IAsyncStream<T>? other) => true;

        public Task<IList<StreamSubscriptionHandle<T>>> GetAllSubscriptionHandles() =>
            Task.FromResult<IList<StreamSubscriptionHandle<T>>>(null);

        public Task OnCompletedAsync() => asyncObserver.OnCompletedAsync();

        public Task OnErrorAsync(Exception ex) => asyncObserver.OnErrorAsync(ex);

        public Task OnNextAsync(T item, StreamSequenceToken token = null) => asyncObserver.OnNextAsync(item, token);

        public Task OnNextBatchAsync(IEnumerable<T> batch, StreamSequenceToken token = null) => Task.CompletedTask;

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer)
        {
            asyncObserver = observer;
            return Task.FromResult<StreamSubscriptionHandle<T>>(null);
        }

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncObserver<T> observer,
            StreamSequenceToken token, StreamFilterPredicate filterFunc = null, object filterData = null)
            => Task.FromResult<StreamSubscriptionHandle<T>>(null);

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T> observer)
            => Task.FromResult<StreamSubscriptionHandle<T>>(null);

        public Task<StreamSubscriptionHandle<T>> SubscribeAsync(IAsyncBatchObserver<T> observer,
            StreamSequenceToken token)
            => Task.FromResult<StreamSubscriptionHandle<T>>(null);
    }
}
