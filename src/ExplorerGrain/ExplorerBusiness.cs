using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using Comax.Commons.Orchestrator.Contracts.UserMetadata;
using CommunAxiom.Commons.Client.AgentClusterRuntime;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pfi = CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.PortfolioItem;
namespace CommunAxiom.Commons.Client.Grains.ExplorerGrain
{
    public class ExplorerBusiness
    {
        private readonly IServiceProvider _serviceProvider;
        public ExplorerBusiness(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<PortfoliosList> GetPortfolio(string? ownerUri = null)
        {
            var f = _serviceProvider.SegregatedOrchClientFactory();
            var res = await f.WithClusterClient(async oc =>
            {
                var reg = oc.GetPortfolioRegistry();
                var ix = await reg.GetIndex(ownerUri);
                return ix;
            });

            return new PortfoliosList
            {
                OwnerUri = ownerUri,
                Portfolios = res.Result.Select(x => new pfi
                {
                    Uri = x.Uri,
                    Description = x.Description,
                    Name = x.Name,
                    OwnerUri = ownerUri,
                    ParentId = x.ParentId,
                    ID = x.Id
                }).ToList()
            };
        }

        internal async Task<List<PortfolioInfo>> ListPortfolios()
        {
            var f = _serviceProvider.SegregatedOrchClientFactory();
            var res = await f.WithClusterClient(async oc =>
            {
                var reg = oc.GetPortfolioRegistry();
                var lst = await reg.ListPortfolios();
                return lst;
            });

            return res;
        }
    }
}
