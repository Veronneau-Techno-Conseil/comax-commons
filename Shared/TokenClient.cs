using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.Configuration;
using IdentityModel.Client;
using IdentityModel;
using System.Security.Claims;

namespace CommunAxiom.Commons.Shared
{
    public class TokenClient
    {
        public const string SCOPES_OFFLINE = "openid offline_access";
        const string WELL_KNOWN = ".well-known/openid-configuration";
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly OIDCSettings _settings;
        public DiscoveryDocumentResponse? TokenMetadata { get; private set; }
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
                //TokenMetadata = await _httpClient.GetFromJsonAsync<TokenMetadata>($"{url}{WELL_KNOWN}");
                TokenMetadata = await _httpClient.GetDiscoveryDocumentAsync(url);
                _configured = true;
            }
        }

        public async Task<(bool, TokenData?)> GetToken(string clientId, string secret, string scope)
        {
            await this.Configure();
            
            var res = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                ClientId = clientId,
                ClientSecret = secret,
                Scope = scope
            });
            if (res.HttpResponse.IsSuccessStatusCode)
            {
                return (true, new TokenData { access_token = res.AccessToken, expires_in = res.ExpiresIn, refresh_token = res.RefreshToken, token_type = res.TokenType });
            }
            else if(res.HttpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return (false, null);
            }
            else
            {
                throw new Exception($"Unexpected result calling token endpoint: {res.HttpStatusCode}=> {res.HttpErrorReason}, {await res.HttpResponse.Content.ReadAsStringAsync()}");
            }
        }

        public async Task<(bool, IEnumerable<Claim>?)> RequestIntrospection(string clientId, string secret, string token)
        {
            await this.Configure();

            var res = await _httpClient.IntrospectTokenAsync(new TokenIntrospectionRequest
            { 
                ClientId = clientId,
                ClientSecret = secret,
                Token = token,
                AuthorizationHeaderStyle = BasicAuthenticationHeaderStyle.Rfc2617
            });
            if (res.HttpResponse.IsSuccessStatusCode)
            {
                return (true, res.Claims);
            }
            else if (res.HttpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return (false, null);
            }
            else
            {
                throw new Exception($"Unexpected result calling token endpoint: {res.HttpStatusCode}=> {res.HttpErrorReason}, {await res.HttpResponse.Content.ReadAsStringAsync()}");
            }
        }
    }
}
