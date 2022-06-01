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
        Task<string> AddAPortfolio(PortfolioDetails portfolio);
        Task<PortfolioDetails> GetAPortfolioDetails(string portfolioID);
        Task<List<PortfolioDetails>> FilterPortfolios(string filter);
    }
}
