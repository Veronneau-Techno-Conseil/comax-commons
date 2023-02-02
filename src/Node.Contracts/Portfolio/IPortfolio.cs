using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using CommunAxiom.Commons.Shared;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio
{
    public interface IPortfolio: IGrainWithStringKey
    {
        Task<OperationResult<List<PortfolioItem>>> ListPortfolioItems();
        Task<OperationResult> Upsert(PortfolioItem item);
        Task<OperationResult> DeleteItem(string uri);
        Task Delete();
    }
}
