using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Ingestor;

namespace CommunAxiom.Commons.Ingestion.Injestor
{
    public interface IIngestor
    {
        Task<IngestorResult> Parse(Stream stream);

        void Configure(IEnumerable<DataSourceConfiguration> fields);
    }
}

