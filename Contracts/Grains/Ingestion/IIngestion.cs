using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion
{
    public interface IIngestion: IGrainWithStringKey
    {
        Task Run();
        Task<IngestionHistory> GetHistory();
    }
}
