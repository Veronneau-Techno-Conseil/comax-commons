using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Services
{
    public class HttpClientConfig : IHttpClientConfig
    {
        private readonly IAccessTokenService _accessTokenService;
        private bool _configured;
        public HttpClientConfig(IAccessTokenService accessTokenService)
        {
            _accessTokenService = accessTokenService;
        }

        public async Task Configure(HttpClient client)
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            if (!string.IsNullOrWhiteSpace(jwtToken))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            _configured = true;
            
        }
    }
}
