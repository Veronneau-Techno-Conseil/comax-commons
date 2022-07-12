using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.IO;
using CommunAxiom.Commons.Ingestion;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    public class Ingestions : Grain, IIngestion
    {
        private readonly Business _business;
        
        public Ingestions(Importer importer, [PersistentState("ingestions")] IPersistentState<SourceState> sourceState)
        {
            _business = new Business(importer, new GrianFactory(this.GrainFactory), this.GrainReference.GrainIdentity.PrimaryKeyString);

        }

        public Task<History> GetHistory()
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
