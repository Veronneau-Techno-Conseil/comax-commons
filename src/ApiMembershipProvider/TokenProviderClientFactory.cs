using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Comax.Commons.CommonsShared.ApiMembershipProvider
{
    public class TokenProviderClientFactory : ISvcClientFactory
    {
        private readonly ApiMembershipConfig _configuration;
        private readonly IServiceProvider _serviceProvider;
        public TokenProviderClientFactory(IOptions<ApiMembershipConfig> apiMembershipConfig, IServiceProvider serviceProvider)
        {
            _configuration = apiMembershipConfig.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task<ApiRef.RefereeSvc> GetRefereeSvc()
        {
            var client = new HttpClient();

            var tp = this._serviceProvider.GetService<ITokenProvider>();
            if(tp != null)
            {
                var token = await tp.FetchToken();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
            ApiRef.RefereeSvc refereeSvc = new ApiRef.RefereeSvc(_configuration.Host, client);
            return refereeSvc;
        }
    }
}
