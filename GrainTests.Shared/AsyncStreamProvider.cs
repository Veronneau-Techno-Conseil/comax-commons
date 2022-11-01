using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrainTests.Shared
{
    public class AsyncStreamProvider<T> : IStreamProvider
    {
        
            private readonly AsyncStreamMock<T> _asyncStream;

            public AsyncStreamProvider()
            {
                _asyncStream = new AsyncStreamMock<T>();
            }

            public string Name => string.Empty;

            public bool IsRewindable => false;

            public IAsyncStream<Message> GetStream<Message>(Guid streamId, string streamNamespace)
                => (IAsyncStream<Message>)_asyncStream;
    }
}
