using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GrainTests.Shared
{
    public class AsyncStreamObserver<T> : IAsyncObserver<T>
    {

            public T Message { get; internal set; }

            public Task OnCompletedAsync() => Task.CompletedTask;

            public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

            public Task OnNextAsync(T item, StreamSequenceToken? token = null)
            {
                Message = item;
                return Task.CompletedTask;
            }
        }
}
