using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Portfolio
{
    public interface IPortfolio: IGrainWithStringKey
    {
        Task<string> TestGrain(string Grain);
        Task<string> CreatePortfolio(PortfolioDetails portfolio);
        Task<PortfolioDetails> GetDetails();
        Task<bool> IsSet();
    }
}
