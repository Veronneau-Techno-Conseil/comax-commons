using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio
{
    public interface IDatasource: IGrainWithStringKey
    {
        Task<DatasourceDetails> GetDatasourceDetails();
        Task Upsert(DatasourceDetails item);
        Task Delete();
    }
}
