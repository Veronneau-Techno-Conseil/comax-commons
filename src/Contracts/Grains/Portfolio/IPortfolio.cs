using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Portfolio
{
    public interface IPortfolio : IGrainWithGuidKey
    {
        Task AddPortfolio(PortfolioItem portfolio);
        Task<IEnumerable<PortfolioItem>> GetPortfoliosList();
        Task<PortfolioItem> GetAPortfolioDetails(Guid portfolioID);
        Task<IEnumerable<PortfolioItem>> FilterPortfolios(string filter);
        Task<bool> CheckIfUnique(string name);
    }
}