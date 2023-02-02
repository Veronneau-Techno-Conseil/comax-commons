using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio
{
    [AuthorizeClaim]
    public interface IPortfolioRegistry: IGrainWithGuidKey
    {
        Task<List<PortfolioInfo>> ListPortfolios();
        Task DeleteAll();
        Task<OperationResult> DeletePortfolioItem(string portfolioItemId);
        Task<OperationResult<List<PortfolioItem>>> GetIndex(string? owner = null);
        Task<OperationResult> UpsertPortfolioItem(PortfolioItem portfolioItem);
    }
}
