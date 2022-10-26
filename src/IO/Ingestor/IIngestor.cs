using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public interface IIngestor
    {
        Task<IngestorResult> ParseAsync(Stream stream);

        void Configure(IEnumerable<FieldMetaData> fields);
    }
}

