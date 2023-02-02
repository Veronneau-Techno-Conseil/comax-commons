using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SharedPortfolio.Portfolio
{
    internal class PortfolioDal
    {
        private bool _read = false;
        private readonly IPersistentState<SharedPortfolioState> _portfolio;
        public PortfolioDal(IPersistentState<SharedPortfolioState> portfolio)
        {
            _portfolio = portfolio;
        }

        private async Task EnsureRead()
        {
            if(!_read)
            {
                await _portfolio.ReadStateAsync();
                if (_portfolio.State.PortfolioItems == null)
                    _portfolio.State.PortfolioItems = new List<PortfolioItem>();
                _read = true;
            }
        }

        public async Task Init(string owner)
        {
            await EnsureRead();
            if (!_portfolio.RecordExists)
            {
                _portfolio.State = new SharedPortfolioState { OwnerId = owner, PortfolioItems = new List<PortfolioItem>() };
                await _portfolio.WriteStateAsync();
                await _portfolio.ReadStateAsync();
            }
        }

        public async Task<List<PortfolioItem>> List()
        {
            await EnsureRead();
            return _portfolio.State.PortfolioItems.ToList();
        }

        public async Task Upsert(PortfolioItem portfolioItem)
        {
            await EnsureRead();
            if (!_portfolio.RecordExists)
                throw new InvalidOperationException("State needs to be initialized first");

            var entry = _portfolio.State.PortfolioItems.FirstOrDefault(x => x.Uri == portfolioItem.Uri);
            
            if(entry != null)
            {
                _portfolio.State.PortfolioItems.Remove(entry);
            }
            _portfolio.State.PortfolioItems.Add(portfolioItem);
            await _portfolio.WriteStateAsync();
        }

        public async Task Delete()
        {
            await EnsureRead();
            await _portfolio.ClearStateAsync();
        }
        public async Task Delete(string uri)
        {
            await EnsureRead();
            var entry = _portfolio.State.PortfolioItems.FirstOrDefault(x => x.Uri == uri);

            if (entry != null)
            {
                _portfolio.State.PortfolioItems.Remove(entry);
                await _portfolio.WriteStateAsync();
            }
        }
    }
}
