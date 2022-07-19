using Newtonsoft.Json.Linq;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Storage
{
    public interface IStorageGrain : IGrainWithStringKey
    {
        Task SaveData(JObject obj);
        Task<List<JObject>> GetData();
    }
}
