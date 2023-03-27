using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.DotnetSdk.Helpers.OIDC;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonsIntegration.Tests
{
    public class TestTokenProvider : ITokenProvider
    {
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
        }

        public async Task<string> FetchToken()
        {
            if (Cluster.NoAuth)
                return "";

            if(_tokens.ContainsKey(_userContract.username))
                return _tokens[_userContract.username];
            var (res,token) = await _tokenClient.AuthenticatePassword(_userContract.username, _userContract.password);
            token.Should().NotBeNull();
            if (token == null)
                return "";
            if (res)
            {
                _tokens[_userContract.username] = token.access_token.ToString();
            }
            return token.access_token;
        }

        private class TestUserContract
        {
            public string username { get; set; }
            public string password { get; set; }
        }
    }
}
