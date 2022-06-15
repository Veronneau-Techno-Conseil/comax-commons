using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;

namespace CommunAxiom.Commons.Ingestion
{
    public sealed class Importer
    {
        private readonly IDataSourceFactory _sourceFactory;
        private readonly IIngestorFactory _ingestionFactory;

        public Importer(IDataSourceFactory sourceFactory, IIngestorFactory ingestorFactory)
        {
            _sourceFactory = sourceFactory;
            _ingestionFactory = ingestorFactory;
        }

        public Task<IngestorResult> Import(SourceConfig sourceConfig, IList<FieldMetaData> fieldMetaDatas)
        {
            var dataSourceReader = _sourceFactory.Create(sourceConfig.DataSourceType);
            dataSourceReader.Setup(sourceConfig);
            var stream = dataSourceReader.ReadData();
            var ingestor = _ingestionFactory.Create(dataSourceReader.IngestorType);
            ingestor.Configure(fieldMetaDatas);
            return ingestor.ParseAsync(stream);
        }
    }
}

