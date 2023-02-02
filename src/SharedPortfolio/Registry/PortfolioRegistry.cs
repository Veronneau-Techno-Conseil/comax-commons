using Comax.Commons.Orchestrator.Contracts.Portfolio;
using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SharedPortfolio.Registry
{
    public class PortfolioRegistry : Grain, IPortfolioRegistry
    {
        private readonly IPersistentState<PortfolioRegistryState> _registry;
        private PortfolioRegistryBusiness _portfolioBusiness;

        public PortfolioRegistry([PersistentState("portfolioRegistryState")] IPersistentState<PortfolioRegistryState> registry)
        {
            _registry = registry;
        }

        public override async Task OnActivateAsync()
        {
            _portfolioBusiness = new PortfolioRegistryBusiness(GrainFactory.AsComax(GetStreamProvider));
            await _portfolioBusiness.Init(_registry);
            base.OnActivateAsync();
        }

        public async Task DeleteAll()
        {
            var uri = this.GetUser().GetOwner();
            await _portfolioBusiness.DeletePortfolio(uri);
        }

        public async Task<OperationResult> DeletePortfolioItem(string portfolioItemId)
        {
            var uri = this.GetUser().GetOwner();
            return await _portfolioBusiness.DeletePortfolioItem(uri, portfolioItemId);
        }

        public async Task<OperationResult<List<PortfolioItem>>> GetIndex(string? owner = null)
        {
            if (owner == null)
                owner = this.GetUser().GetOwner();
            return await _portfolioBusiness.GetIndex(owner);
        }

        public async Task<List<PortfolioInfo>> ListPortfolios()
        {
            return await _portfolioBusiness.ListPortfolios();
        }

        public async Task<OperationResult> UpsertPortfolioItem(PortfolioItem portfolioItem)
        {
            return await _portfolioBusiness.UpsertPortfolioItem(portfolioItem);
        }
    }
}
