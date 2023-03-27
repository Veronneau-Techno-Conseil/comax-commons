using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.OIDC;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.DotnetSdk.Helpers.OIDC;

namespace OrchestratorIntegration.Tests
{
    public class TestTokenProvider : ITokenProvider
    {
        private readonly OIDCSettings _oIDCSettings;
        private readonly TokenClient _tokenClient;
        private readonly IConfiguration _configuration;
        private readonly TestUserContract _userContract;

        private readonly Dictionary<string,string> _tokens = new Dictionary<string,string>();
        public TestTokenProvider(OIDCSettings oIDCSettings, IConfiguration configuration)
        {
            _tokenClient = new TokenClient(oIDCSettings);
            _configuration = configuration;
            _userContract = new TestUserContract();
            _configuration.Bind("TestUser", _userContract);
            _oIDCSettings = oIDCSettings;
        }

        public async Task<string> FetchToken()
        {
            if (Cluster.NoAuth)
                return "";

            TokenData? tokenData = null;
            bool result = false;
            if (Cluster.AsCommonsAgent)
            {
                if (_tokens.ContainsKey(_oIDCSettings.ClientId))
                    return _tokens[_oIDCSettings.ClientId];
                var (res, token) = await _tokenClient.AuthenticateClient(_oIDCSettings.Scopes);
                tokenData = token;
                result = res;
            }
            else
            {
                if (_tokens.ContainsKey(_userContract.username))
                    return _tokens[_userContract.username];
                var (res, token) = await _tokenClient.AuthenticatePassword(_userContract.username, _userContract.password);
                tokenData = token;
                result = res;
            }

            tokenData.Should().NotBeNull();
            if (tokenData == null)
                return "";
            if (result)
            {
                if (Cluster.AsCommonsAgent)
                    _tokens[_oIDCSettings.ClientId] = tokenData.access_token.ToString();
                else
                    _tokens[_userContract.username] = tokenData.access_token.ToString();
            }
            return tokenData.access_token;
        }

        private class TestUserContract
        {
            public string username { get; set; }
            public string password { get; set; }
        }
    }
}
