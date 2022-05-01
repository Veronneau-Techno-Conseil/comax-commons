using System;
using Orleans;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using System.Threading.Tasks;
using Orleans.GrainDirectory;
using Orleans.Runtime;

namespace PortfolioGrain
{
    [GrainDirectory(GrainDirectoryName = "MyGrainDirectory")]
    public class Portfolios: Grain, IPortfolio
    {
        public Portfolios([PersistentState("portfolios")] IPersistentState<PortfolioDetails> portfolioDetails)
        {
            _portfolioDetails = portfolioDetails;
        }

        private readonly IPersistentState<PortfolioDetails> _portfolioDetails;

        public async Task<string> CreatePortfolio(PortfolioDetails portfolio)
        {
            _portfolioDetails.State = portfolio;
            await _portfolioDetails.WriteStateAsync();
            return "The PortfolioID has been created with ID: " + this.IdentityString + " and details: \nPortfolioID:" + _portfolioDetails.State.PortfolioID + "\nName: " + _portfolioDetails.State.PortfolioName;
        }

        Task<string> IPortfolio.TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }

        public async Task<PortfolioDetails> GetDetails()
        {
            await _portfolioDetails.ReadStateAsync();
            return _portfolioDetails.State;
        }

        public async Task<bool> IsSet()
        {
            var res = await GetDetails();
            return res != null && !string.IsNullOrWhiteSpace(res.PortfolioID);
        }
    }
}