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
        Task<OperationResult<AuthorizationInstructions>> LaunchServiceAuthentication(string clientId, string clientSecret, string redirectUri);
        Task<OperationResult<AuthorizationInstructions>> LaunchAuthentication(string redirectUri);
        Task<OperationResult<SessionInfo>> RetrieveToken(string clientId, string clientSecret, string devideCode, int interval);
        Task Proceed();
        Task Complete();
        Task SetResult(BrowserResult browserResult);
        Task<SessionInfo> GetSessionInfo();
    }
}
