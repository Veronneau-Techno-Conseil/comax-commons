using System;
using Orleans;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using System.Threading.Tasks;
using Orleans.GrainDirectory;

namespace PortfolioGrain
{
    [GrainDirectory(GrainDirectoryName = "MyGrainDirectory")]
    public class Portfolios: Grain, IPortfolio
    {
        private PortfolioDetails _portfolioDetails = new PortfolioDetails();

        public Task<string> CreatePortfolio(string GrainID, PortfolioDetails portfolio)
        {
            _portfolioDetails.PortfolioID = portfolio.PortfolioID;
            _portfolioDetails.PortfolioName = portfolio.PortfolioName;

            return Task.FromResult("The PortfolioID has been created with ID: " + GrainID + " and details: \nPortfolioID:" + _portfolioDetails.PortfolioID + "\nName: " + _portfolioDetails.PortfolioName);
        }

        Task<string> IPortfolio.TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}