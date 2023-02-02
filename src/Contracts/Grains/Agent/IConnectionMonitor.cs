using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public interface IConnectionMonitor: IGrainWithGuidKey
    {
        Task EnsureStarted();
        Task<bool> IsConnected();
    }
}
