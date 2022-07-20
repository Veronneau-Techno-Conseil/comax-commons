using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Portfolio
{
    public interface IPortfolio : IGrainWithStringKey
    {
        Task AddAPortfolio(Portfolio portfolio);
        Task<IEnumerable<Portfolio>> GetPortfoliosList();
        Task<Portfolio> GetAPortfolioDetails(string portfolioID);
        Task<IEnumerable<Portfolio>> FilterPortfolios(string filter);
        Task<bool> CheckIfUnique(string name);
    }
}