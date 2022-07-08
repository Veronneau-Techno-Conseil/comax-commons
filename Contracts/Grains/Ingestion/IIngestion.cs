using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion
{
    public interface IIngestion: IGrainWithStringKey
    {
        Task Run();
        Task<History> GetHistory();
    }

    public class History
    {
        // public List<SourceConfig> sourceConfigs { get; set; }
    }
}
