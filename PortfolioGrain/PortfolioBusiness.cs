using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioGrain
{
    public class PortfolioBusiness
    {
        private readonly IConfiguration _configuration;
        private PortfolioRepo _portfolioRepo;
        public PortfolioBusiness(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public void Init(IPersistentState<Portfolio> portDetails, IPersistentState<PortfoliosList> portList)
        {
            _portfolioRepo = new PortfolioRepo(portDetails, portList);
        }

        public async Task AddAPortfolio(Portfolio portfolio)
        {
            if (portfolio != null)
            {
                var listCreated = await CheckPortfoliolist();
                if (listCreated != true)
                {
                    await CreatePortfoliosList();
                }
                await _portfolioRepo.AddAPortfolio(portfolio);
            }
        }

        public async Task<IEnumerable<Portfolio>> GetList()
        {
            var ListCreated = await CheckPortfoliolist();
            if (ListCreated != true)
            {
                await CreatePortfoliosList();
            }
            var portfoliosList = await _portfolioRepo.GetListDetails();
            if (portfoliosList.Portfolios != null)
            {
                return portfoliosList.Portfolios;
            }
            return await Task.FromResult<IEnumerable<Portfolio>>(null);
        }

        public async Task<IEnumerable<Portfolio>> FilterPortfolios(string filter)
        {
            var portfoliosList = await _portfolioRepo.FilterPortfolios(filter);
            if (portfoliosList != null)
            {
                return portfoliosList;
            }
            return await Task.FromResult<List<Portfolio>>(null);
        }

        public async Task<Portfolio> GetAPortfolioDetails(string portfolioID)
        {
            var portfolio = await _portfolioRepo.GetAPortfolioDetails(portfolioID);
            if (portfolio != null)
            {
                return portfolio;
            }
            return await Task.FromResult<Portfolio>(null);
        }

        public async Task<bool> CheckPortfoliolist()
        {
            return await _portfolioRepo.ListIsSet();
        }

        public async Task<PortfoliosList> CreatePortfoliosList()
        {
            var listCreated = await _portfolioRepo.CreateList();
            if (listCreated.Portfolios != null)
            {
                return listCreated;
            }
            return await Task.FromResult<PortfoliosList>(null);
        }

        public async Task<bool> CheckIfUnique(string name)
        {
            if (name != null)
            {
                return await _portfolioRepo.CheckIfUnique(name);
            }
            return false;
        }
    }
}