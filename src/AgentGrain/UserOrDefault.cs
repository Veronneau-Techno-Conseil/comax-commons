using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Orleans.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain
{
    public class UserOrDefault : IUserContextAccessor
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly AppIdProvider _appIdProvider;
        
        public bool IsCluster { get; private set; }
        public string Uri
        {
            get;
            private set;
        }
        
        public string Token
        {
            get;
            private set;
        }

        public UserOrDefault(ITokenProvider tokenProvider, AppIdProvider appIdProvider) 
        {
            _tokenProvider = tokenProvider;
            _appIdProvider = appIdProvider;            
        }

        public async Task<UserOrDefault> Init()
        {
            Token = await _tokenProvider?.FetchToken();
            if (string.IsNullOrWhiteSpace(Token))
            {
                Token = await _appIdProvider.GetAccessToken();
                IsCluster = true;
            }

            if (IsCluster)
            {
                Uri = (await _appIdProvider.GetClaims()).GetUri();
            }
            else
            {
                Uri = this.GetUser().GetUri();
            }

            return this;
        }

        
    }
}
