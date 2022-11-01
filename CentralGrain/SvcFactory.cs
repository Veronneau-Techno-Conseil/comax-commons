using Comax.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.CentralGrain
{
    public class SvcFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ITokenProvider _tokenProvider;
        public SvcFactory(IServiceProvider serviceProvider, IConfiguration configuration, ITokenProvider tokenProvider)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _tokenProvider = tokenProvider;
        }
        public async Task<CentralApi.CentralSvc> GetService()
        {

            var url = _configuration["CentralApiUrl"];
            var client = _serviceProvider.GetService<HttpClient>();
            var token = await _tokenProvider.FetchToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var svc = new CentralApi.CentralSvc(url, client);
            return svc;
        }
    }
}
