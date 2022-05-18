using CommunAxiom.Commons.Client.Contracts.ComaxSystem;

namespace CommunAxiom.Commons.ClientUI.Server.Hubs.Interfaces
{
    public interface ISystemChannel: IDisposable
    {
        Task Emit(SystemEvent systemEvent);
        Task<SystemEvent> Consume(CancellationToken token);
    }
}
