using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Orleans.Messaging;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.CommonsShared.ApiMembershipProvider
{
    public class ApiGatewayListProvider : IGatewayListProvider
    {
        private readonly ISvcClientFactory _refereeClientFact;
        private readonly ApiMembershipConfig _apiMembershipClientConfig;
        private DateTime _lastFetch = DateTime.MinValue;
        private List<Uri> _uris = new List<Uri>();
        public TimeSpan MaxStaleness { get; private set; }
        private DateTime RefreshDue {get
            {
                return _lastFetch + MaxStaleness;
            }
        }

        public bool IsUpdatable => true;

        public ApiGatewayListProvider(IOptions<ApiMembershipConfig> configuration, ISvcClientFactory clientFactory)
        {
            _refereeClientFact = clientFactory;
            _apiMembershipClientConfig = configuration.Value;
            MaxStaleness = TimeSpan.Parse(_apiMembershipClientConfig.CacheDuration);
        }

        public async Task<IList<Uri>> GetGateways()
        {
            if (DateTime.UtcNow > this.RefreshDue)
            {
                var svc = await _refereeClientFact.GetRefereeSvc();
                var lst = await svc.ReadAllAsync();
                _uris = lst.Members.Where(x=>x.Item1.Status == (int)SiloStatus.Active).Select(x => x.Item1.SiloAddress.GetUri(x.Item1.ProxyPort)).ToList();
                _lastFetch = DateTime.UtcNow;
            }
            return _uris;
        }

        public Task InitializeGatewayListProvider()
        {
            return Task.CompletedTask;
        }
    }
}
