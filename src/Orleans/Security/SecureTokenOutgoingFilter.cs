using Comax.Commons.Shared.OIDC;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Orleans.Security
{
    public class SecureTokenOutgoingFilter : IOutgoingGrainCallFilter
    {
        private readonly ILogger<SecureTokenOutgoingFilter> _logger;
        private readonly ITokenProvider _tokenProvider;

        public SecureTokenOutgoingFilter(ILogger<SecureTokenOutgoingFilter> logger, ITokenProvider tokenProvider)
        {
            _logger = logger;
            _tokenProvider = tokenProvider;
        }

        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            var token = await _tokenProvider.FetchToken();
            if (string.IsNullOrEmpty(token))
            {
                RequestContext.Remove(Config.SECURE_TOKEN_KEY);
            }
            else
            {
                RequestContext.Set(Config.SECURE_TOKEN_KEY, token);
            }

            await context.Invoke();
        }                                               
    }
}
