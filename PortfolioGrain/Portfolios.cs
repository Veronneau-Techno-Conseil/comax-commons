using System;
using Orleans;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using System.Threading.Tasks;
using Orleans.GrainDirectory;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioGrain
{
    [GrainDirectory(GrainDirectoryName = "MyGrainDirectory")]
    public class Portfolios : Grain, IPortfolio
    {
        private readonly IPersistentState<Portfolio> _portfolioDetails;
        private readonly IPersistentState<PortfoliosList> _portfolioList;

        public Portfolios(
            [PersistentState("portfolios")] IPersistentState<Portfolio> portfolioDetails,
            [PersistentState("portfoliosList")] IPersistentState<PortfoliosList> portfoliosList)
        {
            _portfolioDetails = portfolioDetails;
            _portfolioList = portfoliosList;
        }

        public async Task<string> CreatePortfoliosList()
        {
            var portfoliosList = new PortfoliosList();
            _portfolioList.State = portfoliosList;
            await _portfolioList.WriteStateAsync();
            return "The PortfoliosList Grain has been created with ID: " + this.IdentityString;
        }

        public async Task<PortfoliosList> GetListDetails()
        {
            await _portfolioList.ReadStateAsync();
            return _portfolioList.State;
        }

        public async Task<bool> ListIsSet()
        {
            var res = await GetListDetails();
            return res.Portfolios != null; //&& List !string.IsNullOrWhiteSpace(res.);
        }

        public async Task<string> AddAPortfolio(Portfolio portfolio)
        {
            var portfoliosList = await GetListDetails();
            if (portfoliosList.Portfolios == null)
            {
                portfoliosList.Portfolios = new List<Portfolio>();
            }
            portfoliosList.Portfolios.Add(portfolio);
            await _portfolioList.WriteStateAsync();
            return "New Portfolio Added";
        }

        public async Task<Portfolio> GetAPortfolioDetails(string portfolioID)
        {
            var PortfolioList = await GetListDetails();
            var Portfolio = PortfolioList.Portfolios.Where(x => x.ID == portfolioID).FirstOrDefault();
            return (Portfolio);
        }

        public async Task<List<Portfolio>> FilterPortfolios(string filter)
        {
            var PortfolioList = await GetListDetails();
            var FilteredList = (PortfolioList.Portfolios.Where(x => x.Name.Contains(filter)).ToList());
            return (FilteredList);
        }
    }
}