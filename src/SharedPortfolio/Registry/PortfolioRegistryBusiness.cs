using Comax.Commons.Orchestrator.Contracts.Portfolio;
using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SharedPortfolio.Registry
{
    public class PortfolioRegistryBusiness: IUserContextAccessor
    {
        private readonly IComaxGrainFactory _comaxGrainFactory;
        private PortfolioRegistryDal _dal;
        public PortfolioRegistryBusiness(IComaxGrainFactory comaxGrainFactory)
        {
            _comaxGrainFactory = comaxGrainFactory;
        }

        public async Task Init(IPersistentState<PortfolioRegistryState> registry)
        {
            _dal = new PortfolioRegistryDal(registry);
            await _dal.EnsureRead();
        }

        public async Task DeletePortfolio(string uri)
        {
            var portfolio = _comaxGrainFactory.GetGrain<IPortfolio>(uri);
            await portfolio.Delete();
            await _dal.Remove(uri);
        }

        public Task<OperationResult> DeletePortfolioItem(string user, string uri)
        {
            var portfolio = _comaxGrainFactory.GetGrain<IPortfolio>(user);
            return portfolio.DeleteItem(uri);
        }

        public Task<OperationResult<List<PortfolioItem>>> GetIndex(string owner)
        {
            var portfolio = _comaxGrainFactory.GetGrain<IPortfolio>(owner);
            return portfolio.ListPortfolioItems();
        }

        public async Task<List<PortfolioInfo>> ListPortfolios()
        {
            var lst = await _dal.List();
            return lst;
        }

        public async Task<OperationResult> UpsertPortfolioItem(PortfolioItem portfolioItem)
        {
            var pi = new PortfolioInfo()
            {
                OwnerUri = this.GetUser().GetOwner(),
                PortfolioUri = portfolioItem.PortfolioUri,
                OwnerUsername = this.GetUser().GetOwnerDisplayName()
            };

            await _dal.Upsert(pi);
            var portfolio = _comaxGrainFactory.GetGrain<IPortfolio>(portfolioItem.PortfolioUri);
            return await portfolio.Upsert(portfolioItem);
        }
    }
}
