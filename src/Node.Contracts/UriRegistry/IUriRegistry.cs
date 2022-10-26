using Orleans;
using System;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.UriRegistry
{
    public interface IUriRegistry: IGrainWithStringKey
    {
        Task<Guid> GetOrCreate();
        Task<UserTuple> GetCurrentUser();
    }
}
