using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;

namespace CommunAxiom.Commons.Ingestion
{
    public class Importer
    {
        private readonly ISourceFactory _sourceFactory;
        private readonly IngestorFactory _ingestionFactory;

        public Importer(ISourceFactory sourceFactory, IngestorFactory ingestorFactory)
        {
            _sourceFactory = sourceFactory;
            _ingestionFactory = ingestorFactory;
        }

        public IngestorResult Import(DataSourceType sourceType, DataSourceConfiguration config)
        {
            var dataSourceReader = _sourceFactory.Create(sourceType);
            var stream = dataSourceReader.ReadData();
            var ingestor = _ingestionFactory.Create(dataSourceReader.IngestionType);
            return ingestor.Parse(stream);
        }
    }
}

