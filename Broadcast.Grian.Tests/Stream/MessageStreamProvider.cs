using Orleans.Streams;
using System.Drawing;

namespace Broadcast.Grian.Tests.Stream
{
    public class MessageStreamProvider : IStreamProvider
    {
        private readonly MessageAsyncStream _messageAsyncStream;

        public MessageStreamProvider()
        {
            _messageAsyncStream = new MessageAsyncStream();
        }

        public string Name => string.Empty;

        public bool IsRewindable => false;

        public IAsyncStream<Message> GetStream<Message>(Guid streamId, string streamNamespace)
            => (IAsyncStream<Message>)_messageAsyncStream;

    }

}
