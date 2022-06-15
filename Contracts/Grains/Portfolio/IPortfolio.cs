using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Portfolio
{
    public interface IPortfolio: IGrainWithStringKey
    {
        Task<string> CreatePortfoliosList();
        Task<PortfoliosList> GetListDetails();
        Task<bool> ListIsSet();
        Task<string> AddAPortfolio(Portfolio portfolio);
        Task<Portfolio> GetAPortfolioDetails(string portfolioID);
        Task<List<Portfolio>> FilterPortfolios(string filter);
    }
}
