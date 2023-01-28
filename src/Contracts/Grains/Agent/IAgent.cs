using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public interface IAgent: IGrainWithGuidKey
    {
        Task IAmAlive();
        Task EnsureStarted();
        Task<UserAuthState> GetCommonsAuthState();
        Task<UserAuthState> GetCurrentUserAuthState();
    }
}
