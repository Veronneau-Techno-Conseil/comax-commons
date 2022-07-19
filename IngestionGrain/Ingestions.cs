using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Ingestion;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    public class Ingestions : Grain, IIngestion
    {
        private readonly Business _business;
        
        public Ingestions(Importer importer, [PersistentState("ingestion-history")] IPersistentState<IngestionHistory> history)
        {
            _business = new Business(importer, new GrianFactory(this.GrainFactory), this.GrainReference.GrainIdentity.PrimaryKeyString);
            _business.Init(history);
        }

        public Task<IngestionHistory> GetHistory()
        {
            return _business.GetHistory();
        }

        public Task Run()
        {
            _ = _business.Run();
            return Task.CompletedTask;
        }

    }
}
