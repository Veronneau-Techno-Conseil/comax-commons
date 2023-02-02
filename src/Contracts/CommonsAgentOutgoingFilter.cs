using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Orleans.Security;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public class CommonsAgentOutgoingFilter : IOutgoingGrainCallFilter
    {
        private readonly ILogger<SecureTokenOutgoingFilter> _logger;
        private readonly ITokenProvider _tokenProvider;
        private readonly AppIdProvider _appIdProvider;

        public CommonsAgentOutgoingFilter(ILogger<SecureTokenOutgoingFilter> logger, ITokenProvider tokenProvider, AppIdProvider appIdProvider)
        {
            _logger = logger;
            _tokenProvider = tokenProvider;
            _appIdProvider = appIdProvider;
        }

        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            if (string.IsNullOrWhiteSpace((string)RequestContext.Get(Config.SECURE_TOKEN_KEY)))
            {
                var token = await _tokenProvider.FetchToken();
                if (string.IsNullOrEmpty(token))
                {
                    token = await _appIdProvider.GetAccessToken();
                }
                if (string.IsNullOrEmpty(token))
                {
                    RequestContext.Remove(Config.SECURE_TOKEN_KEY);
                }
                else
                {
                    RequestContext.Set(Config.SECURE_TOKEN_KEY, token);
                }
            }

            await context.Invoke();
        }
    }
}