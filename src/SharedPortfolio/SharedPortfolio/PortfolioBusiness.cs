using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.Validations;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SharedPortfolio.Portfolio
{
    public class PortfolioBusiness: IUserContextAccessor
    {
        private PortfolioDal _portfolioDal;
        private string _id;
        public PortfolioBusiness(string id) 
        {
            _id = id;
        }

        public async Task Init(IPersistentState<SharedPortfolioState> state)
        {
            _portfolioDal = new PortfolioDal(state);
            await _portfolioDal.Init(this.GetUser().GetUri());
        }

        public async Task<List<PortfolioItem>> List()
        {
            return await _portfolioDal.List();
        }

        public async Task<OperationResult> Upsert(PortfolioItem portfolioItem)
        {
            return await portfolioItem.Validatable()
                .NotNull(x => x.Uri)
                .NotNull(y => y.Name)
                .ExecIfValid(async o =>
                {
                    o.PortfolioUri = _id;
                    await _portfolioDal.Upsert(o);
                });
        }

        public async Task<OperationResult> Delete(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return new OperationResult { IsError = true, Error = OperationResult.ERR_UNEXP_NULL, Detail = "parameter null" };
            }

            await _portfolioDal.Delete(uri);
            return new OperationResult { IsError = false };
        }

        public async Task Delete()
        {
            await _portfolioDal.Delete();
        }
    }
}
