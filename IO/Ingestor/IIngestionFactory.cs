using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Injestor;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public interface IIngestionFactory
    {
        IIngestor Create(IngestionType ingestionType);
    }
}

