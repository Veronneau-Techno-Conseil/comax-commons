using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;

namespace CommunAxiom.Commons.Ingestion
{
    public class Importer
    {
        private readonly IDataSourceFactory _sourceFactory;
        private readonly IngestorFactory _ingestionFactory;

        public Importer(IDataSourceFactory sourceFactory, IngestorFactory ingestorFactory)
        {
            _sourceFactory = sourceFactory;
            _ingestionFactory = ingestorFactory;
        }

        public IngestorResult Import(SourceConfig sourceConfig)
        {
            var dataSourceReader = _sourceFactory.Create(sourceConfig.DataSourceType);
            dataSourceReader.Setup(sourceConfig);
            var stream = dataSourceReader.ReadData();
            var ingestor = _ingestionFactory.Create(dataSourceReader.IngestionType);
            return ingestor.Parse(stream);
        }
    }
}

