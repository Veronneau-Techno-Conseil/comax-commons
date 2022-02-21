using System;
using Orleans;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using System.Threading.Tasks;

namespace PortfolioGrain
{
    public class Portfolios: Grain, IPortfolio
    {

        Task<string> IPortfolio.TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}
