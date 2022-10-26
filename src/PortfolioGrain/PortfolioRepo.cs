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
        private readonly IPersistentState<Portfolio> _portfolioDetails;
        private readonly IPersistentState<PortfoliosList> _portfolioList;
        public PortfolioRepo(IPersistentState<Portfolio> portfolioDetails,
            IPersistentState<PortfoliosList> portfoliosList)
        {
            _portfolioDetails = portfolioDetails;
            _portfolioList = portfoliosList;
        }

        public async Task<bool> ListIsSet()
        {
            var res = await GetListDetails();
            return res != null;
        }

        public async Task<PortfoliosList> GetListDetails()
        {
            await _portfolioList.ReadStateAsync();
            return _portfolioList.State;
        }

        public async Task AddAPortfolio(Portfolio portfolio)
        {
            var portfoliosList = await GetListDetails();
            if (portfoliosList.Portfolios == null)
            {
                portfoliosList.Portfolios = new List<Portfolio>();
            }
            portfoliosList.Portfolios = portfoliosList.Portfolios.Concat(new[] { portfolio });
            await _portfolioList.WriteStateAsync();
        }

        public async Task<PortfoliosList> CreateList()
        {
            var portfoliosList = new PortfoliosList();
            _portfolioList.State = portfoliosList;
            await _portfolioList.WriteStateAsync();
            var listDetails = await GetListDetails();
            return listDetails;
        }

        public async Task<Portfolio> GetAPortfolioDetails(Guid portfolioID)
        {
            var PortfolioList = await GetListDetails();
            var Portfolio = PortfolioList.Portfolios.AsQueryable().Where(x => x.ID == portfolioID).FirstOrDefault();
            return (Portfolio);
        }

        public async Task<IEnumerable<Portfolio>> FilterPortfolios(string filter)
        {
            var portfoliosList = await GetListDetails();
            var filteredList = (portfoliosList.Portfolios.AsQueryable().Where(x => x.Name.Contains(filter)).ToList());
            return (filteredList);
        }

        public async Task<bool> CheckIfUnique(string name)
        {
            var portfoliosList = await GetListDetails();
            if (portfoliosList.Portfolios!=null)
            {
                var filteredPortfolios = portfoliosList.Portfolios.AsQueryable().Where(x => x.Name.Equals(name));
                return !(filteredPortfolios != null && filteredPortfolios.Any());
            }
            else
            {
                return true;
            }
        }
    }
}