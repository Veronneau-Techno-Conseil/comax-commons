using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioGrain
{
    public class PortfolioRepo
    {
        bool _read = false;
        private readonly IPersistentState<PortfoliosList> _portfolioList;
        public PortfolioRepo(IPersistentState<PortfolioItem> portfolioDetails,
            IPersistentState<PortfoliosList> portfoliosList)
        {
            _portfolioList = portfoliosList;
        }

        private async Task EnsureRead()
        {
            if (!_read)
            {
                await _portfolioList.ReadStateAsync();
                if(!_portfolioList.RecordExists)
                    _portfolioList.State = new PortfoliosList();

                if (_portfolioList.State.Portfolios == null)
                    _portfolioList.State.Portfolios = new List<PortfolioItem>();

                _read = true;
            }
        }

        public async Task AddPortfolio(PortfolioItem portfolio)
        {
            await EnsureRead();
            _portfolioList.State.Portfolios.Add(portfolio);
            await _portfolioList.WriteStateAsync();
        }

        public async Task<PortfolioItem> GetDetails(Guid portfolioID)
        {
            await EnsureRead();
            var portfolio = _portfolioList.State.Portfolios.Where(x => x.ID == portfolioID).FirstOrDefault();
            return portfolio;
        }

        public async Task<IEnumerable<Guid>> GetIds()
        {
            await EnsureRead();
            return _portfolioList.State.Portfolios.Select(x => x.ID).ToArray();
        }

        public async Task<IEnumerable<PortfolioItem>> GetList()
        {
            await EnsureRead();
            return _portfolioList.State.Portfolios.ToArray();
        }

        public async Task<IEnumerable<PortfolioItem>> FilterPortfolios(string filter)
        {
            await EnsureRead();

            var filteredList = _portfolioList.State.Portfolios.AsQueryable().Where(x => x.Name.Contains(filter)).ToList();
            return filteredList;
        }

        public async Task<bool> CheckIfUnique(string name)
        {
            await EnsureRead();
            
            var filteredPortfolios = _portfolioList.State.Portfolios.Where(x => x.Name.Equals(name));
            return filteredPortfolios == null || filteredPortfolios.Any();
        }
    }
}