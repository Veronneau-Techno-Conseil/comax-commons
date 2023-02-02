using CommunAxiom.Commons.Client.Contracts.Remote;
using CommunAxiom.Commons.Shared;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Auth
{
    public interface IAuthentication : IGrainWithGuidKey
    {
        Task<OperationResult<AuthorizationInstructions>> LaunchAuthentication(string redirectUri);
        Task Proceed();
        Task Complete();
        Task SetResult(BrowserResult browserResult);
        Task<SessionInfo> GetSessionInfo();
    }
}
