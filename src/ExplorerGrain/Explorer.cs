using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using CommunAxiom.Commons.Client.Contracts.Grains.Explorer;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.Client.Grains.ExplorerGrain;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExplorerGrain
{
    public class Explorer : Grain, IExplorer
    {
        private ExplorerBusiness _explorerBusiness;

        public override Task OnActivateAsync()
        {
            _explorerBusiness = new ExplorerBusiness(this.ServiceProvider);
            return Task.CompletedTask;
        }

        public async Task<PortfoliosList> GetPortfolio(string ownerUri)
        {
            return await _explorerBusiness.GetPortfolio(ownerUri);
        }

        public async Task<List<SharedPortfolio>> GetPortfolios()
        {
            return (await _explorerBusiness.ListPortfolios()).Select(x => new SharedPortfolio { OwnerUri = x.OwnerUri, PortfolioUri = x.PortfolioUri, OwnerUsername = x.OwnerUsername }).ToList();
        }
    }
}
