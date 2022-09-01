using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Ingestion;
using CommunAxiom.Commons.Orleans;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    [Reentrant]
    public class Ingestions : Grain, IIngestion
    {
        private readonly Business _business;

        public Ingestions(Importer importer, [PersistentState("ingestion-history")] IPersistentState<IngestionHistory> history)
        {
            _business = new Business(importer, new GrainFactory(this.GrainFactory), this.GrainReference.GrainIdentity.PrimaryKeyString);
            _business.Init(history);
        }

        public Task<IngestionHistory> GetHistory()
        {
            return _business.GetHistory();
        }

        public Task<IngestionState> Run()
        {
            return _business.Run();
        }

    }
}
