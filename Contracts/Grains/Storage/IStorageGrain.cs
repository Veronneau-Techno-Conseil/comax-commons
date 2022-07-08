using Newtonsoft.Json.Linq;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Storage
{
    public interface IStorageGrain : IGrainWithStringKey
    {
        Task SaveData(JObject obj);
    }
}
