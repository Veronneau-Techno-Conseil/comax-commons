using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public interface IIngestor
    {
        Task<IngestorResult> Parse(Stream stream);

        void Configure(IEnumerable<DataSourceConfiguration> configurations);
    }
}

