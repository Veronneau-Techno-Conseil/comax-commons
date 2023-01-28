using System.Threading.Tasks;
using Orleans;

namespace CommunAxiom.Commons.Client.Contracts.Grains.DateStateMonitorSupervisor
{
    public interface IDateStateMonitorSupervisor : IGrainWithGuidKey
    {
        Task RegisterAsync(string grainKey);
        Task UnregisterAsync(string grainKey);
    }
}