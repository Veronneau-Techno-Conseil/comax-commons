using System.Threading.Tasks;
using Orleans;

namespace CommunAxiom.Commons.Client.Contracts.Grains.DataStateMonitor
{
    public interface IDataStateMonitor : IGrainWithStringKey
    {
        /// <summary>
        /// Stop timer
        /// </summary>
        /// <returns></returns>
        Task UnregisterAsync();
        Task EnsureActive();
    }
}