using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public interface IIngestorFactory
    {
        IIngestor Create(IngestorType ingestorType);
    }
}

