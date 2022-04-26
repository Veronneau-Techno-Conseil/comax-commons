using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Auth
{
    public interface IAuthentication : IGrainWithGuidKey
    {
        Task<OperationResult<AuthorizationInstructions>> LaunchAuthentication(string clientId, string clientSecret);
        Task<OperationResult<SessionInfo>> RetrieveToken(string clientId, string clientSecret, string devideCode, int interval);
    }
}
