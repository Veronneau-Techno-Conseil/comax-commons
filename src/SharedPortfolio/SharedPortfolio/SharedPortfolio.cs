using Comax.Commons.Orchestrator.Contracts.Portfolio;
using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using CommunAxiom.Commons.Shared;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SharedPortfolio.Portfolio
{
    //TODO authorize and verify ownership
    public class SharedPortfolio : Grain, IPortfolio
    {
        private readonly IPersistentState<SharedPortfolioState> _portfolio;
        private PortfolioBusiness _portfolioBusiness;
        public SharedPortfolio([PersistentState("sahredPortfolioState")] IPersistentState<SharedPortfolioState> portfolio) 
        {
            _portfolio = portfolio;
        }

        public override async Task OnActivateAsync()
        {
            _portfolioBusiness = new PortfolioBusiness(this.GetPrimaryKeyString());
            await _portfolioBusiness.Init(_portfolio);
            await base.OnActivateAsync();
        }

        public async Task<OperationResult> DeleteItem(string uri)
        {
            return await _portfolioBusiness.Delete(uri);
        }

        public async Task Delete()
        {
            await _portfolioBusiness.Delete();
        }

        public async Task<OperationResult<List<PortfolioItem>>> ListPortfolioItems()
        {
            //TODO authorize access
            return new OperationResult<List<PortfolioItem>> { Result = await _portfolioBusiness.List() };
        }

        public async Task<OperationResult> Upsert(PortfolioItem item)
        {
            return await _portfolioBusiness.Upsert(item);
        }
    }
}
