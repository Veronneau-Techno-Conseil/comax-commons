using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry
{
    public interface IIdRegistry: IGrainWithGuidKey
    {
        Task<UserTuple> Get();
        Task Set(UserTuple value);
    }
}
