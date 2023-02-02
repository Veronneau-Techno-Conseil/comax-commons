using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public interface IAgtSyncWorker: IGrainWithGuidKey
    {
        Task IAmAlive(string token, Guid id, string uri);
        Task<bool?> IsAuthorized();
        Task Stop();
    }
}
