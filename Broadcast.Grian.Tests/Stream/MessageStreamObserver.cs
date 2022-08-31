using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;

namespace Broadcast.Grian.Tests.Stream
{
    public class MessageStreamObserver : IAsyncObserver<Message>
    {
        public Message Message { get; internal set; }

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex) => Task.CompletedTask;

        public Task OnNextAsync(Message item, StreamSequenceToken? token = null)
        {
            Message = item;
            return Task.CompletedTask;
        }
    }


}
