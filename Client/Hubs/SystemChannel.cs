using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.ClientUI.Server.Hubs.Interfaces;

namespace CommunAxiom.Commons.ClientUI.Server.Hubs
{
    public class SystemChannel: ISystemChannel
    {
        System.Threading.Channels.Channel<SystemEvent> _eventChannel;
        
        public SystemChannel()
        {
            _eventChannel = System.Threading.Channels.Channel.CreateUnbounded<SystemEvent>();
        }

        public Task<SystemEvent> Consume(CancellationToken token)
        {
            return _eventChannel.Reader.ReadAsync(token).AsTask();
        }

        public void Dispose()
        {
            _eventChannel.Writer.Complete();
        }

        public async Task Emit(SystemEvent systemEvent)
        {
            await _eventChannel.Writer.WriteAsync(systemEvent);
        }
    }
}
