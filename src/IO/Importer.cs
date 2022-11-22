using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;
using Newtonsoft.Json.Linq;

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

            var resultValidation = dataSourceReader.ValidateConfiguration();
            if (resultValidation != null && resultValidation.Any())
            {
                var ingestorResult = new IngestorResult();
                foreach (var result in resultValidation)
                {
                    ingestorResult.Errors.Add((null, result)!);
                }
                return Task.FromResult(ingestorResult);
            }

            var stream = dataSourceReader.ReadData();
            var ingestor = _ingestionFactory.Create(dataSourceReader.IngestorType);
            ingestor.Configure(fieldMetaDatas);
            return ingestor.ParseAsync(stream);
        }

        public void Configure(SourceConfig sourceConfig)
        {
            var dataSourceReader = _sourceFactory.Create(sourceConfig.DataSourceType);
            dataSourceReader.Setup(sourceConfig);

            sourceConfig.Configurations.Clear();
            
            foreach (var kvp in dataSourceReader.Configurations)
            {
                sourceConfig.Configurations.Add(kvp.Key, kvp.Value);
            }
        }
    }
}

