using System;
using Orleans;
using System.Threading.Tasks;
using Orleans.GrainDirectory;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Linq;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using Microsoft.Extensions.Configuration;

namespace PortfolioGrain
{
    public class Portfolios : Grain, IPortfolio
    {
        private readonly PortfolioBusiness _portfolioBusiness;
        public Portfolios(IConfiguration configuration, PortfolioBusiness portfolioBusiness, 
            [PersistentState("portfolios")] IPersistentState<Portfolio> portDetails,
            [PersistentState("portfoliosList")] IPersistentState<PortfoliosList> portList)
        {
            _portfolioBusiness = portfolioBusiness;
            _portfolioBusiness.Init(portDetails, portList);
        }

        public async Task AddAPortfolio(Portfolio portfolio)
        {
            await _portfolioBusiness.AddAPortfolio(portfolio);
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosList()
        {
            return await _portfolioBusiness.GetList();
        }

        public async Task<IEnumerable<Portfolio>> FilterPortfolios(string filter)
        {
            return await _portfolioBusiness.FilterPortfolios(filter);
        }

        public async Task<Portfolio> GetAPortfolioDetails(string portfolioID)
        {
            return await _portfolioBusiness.GetAPortfolioDetails(portfolioID);
        }

        public async Task<bool> CheckIfUnique(string name)
        {
            return await _portfolioBusiness.CheckIfUnique(name);
        }
    }
}