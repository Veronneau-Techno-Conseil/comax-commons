using Orleans;
using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Portfolio;

namespace Portfolio
{
    public class Portfolio: Grain, IPortfolio
    {
        public Task<string> TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}
