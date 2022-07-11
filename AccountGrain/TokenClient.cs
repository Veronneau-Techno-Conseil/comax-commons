using CommunAxiom.Commons.Client.Contracts.Configuration;
using CommunAxiom.Commons.Client.Grains.AccountGrain.InternalData;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class TokenClient
    {
        public const string SCOPES_OFFLINE = "openid offline_access";
        const string WELL_KNOWN = ".well-known/openid-configuration";
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly OIDCSettings _settings;
        public TokenMetadata TokenMetadata { get; private set; }
        private bool _configured;
        public TokenClient(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._settings = new OIDCSettings();
            _configuration.Bind(Sections.OIDCSection, this._settings);
            this._httpClient = new HttpClient();
        }

        public async Task Configure()
        {
            if (!_configured)
            {
                string url = _settings.Authority.TrimEnd('/') + '/';
                TokenMetadata = await _httpClient.GetFromJsonAsync<TokenMetadata>($"{url}{WELL_KNOWN}");
                _configured = true;
            }
        }

        public async Task<(bool, TokenData)> GetToken(string clientId, string secret, string scope)
        {
            await this.Configure();
            HttpContent httpContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", secret),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", scope)
            });
            var res = await _httpClient.PostAsync(TokenMetadata.token_endpoint, httpContent);
            if (res.IsSuccessStatusCode)
            {
                return (true, await res.Content.ReadFromJsonAsync<TokenData>());
            }
            else if(res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return (false, null);
            }
            else
            {
                throw new Exception($"Unexpected result calling token endpoint: {res.StatusCode}=> {res.ReasonPhrase}, {await res.Content.ReadAsStringAsync()}");
            }
        }
    }
}
