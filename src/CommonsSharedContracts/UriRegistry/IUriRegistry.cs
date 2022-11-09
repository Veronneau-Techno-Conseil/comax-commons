using Orleans;
using System;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry
{
    public interface IUriRegistry: IGrainWithStringKey
    {
        Task<Guid> GetOrCreate();
        Task<UserTuple> GetCurrentUser();
    }
}
