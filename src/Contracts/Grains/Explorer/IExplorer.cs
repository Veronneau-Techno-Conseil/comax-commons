using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Explorer
{
    public interface IExplorer: IGrainWithGuidKey
    {
        Task<List<SharedPortfolio>> GetPortfolios();
        Task<PortfoliosList> GetPortfolio(string ownerUri);
    }
}
