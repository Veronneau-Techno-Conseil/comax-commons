using CommunAxiom.Commons.Client.Contracts.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Services.Portfolios
{
    public interface IPortfolioService
    {
        public Task<Portfolio> GetPortfolioDetails(string portfolioID);
        public Task<PortfoliosList> GetAllPortfolios();
        public Task CreatePortfolio(Portfolio portfolio);
        public string JustTest();
    }
}
