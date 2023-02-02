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
        private Business _business;
        private readonly Importer _importer;
        private readonly IPersistentState<IngestionHistory> _history;
        
        public Ingestions(Importer importer, [PersistentState("ingestion-history")] IPersistentState<IngestionHistory> history)
        {
            _importer = importer;
            _history = history;
        }

        public Task<IngestionHistory> GetHistory()
        {
            return _business.GetHistory();
        }

        public Task<IngestionState> Run()
        {
            return _business.Run();
        }

        public override Task OnActivateAsync()
        {
            _business = new Business(_importer, new GrainFactory(this.GrainFactory, this.GetStreamProvider), this.GrainReference.GrainIdentity.PrimaryKeyString);
            _business.Init(_history);
            return base.OnActivateAsync(); 
        }
    }
}
