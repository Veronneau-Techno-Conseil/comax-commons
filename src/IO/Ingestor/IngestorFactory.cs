using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public class IngestorFactory : IIngestorFactory
    {
        private readonly Func<IngestorType, IIngestor> _serviceResolver;

        public IngestorFactory(Func<IngestorType, IIngestor> serviceResolver)
        {
            _serviceResolver = serviceResolver;
        }

        public IIngestor Create(IngestorType ingestorType)
        {

            var ingestor = _serviceResolver(ingestorType);

            if (ingestor == null)
            {
                throw new NullReferenceException("No Ingestor resolved!");
            }
            
            return ingestor;
        }
    }
}

