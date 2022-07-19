using CommunAxiom.Commons.Client.Contracts.IO;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Datasource
{
    public interface IDatasource: IGrainWithStringKey
    {
        Task<SourceState> GetState();
        Task<string> TestGrain(string Grain);
    }

}
