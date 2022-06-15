using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    public interface IDataSourceReader
    {
        IngestorType IngestorType { get; }

        IList<DataSourceConfiguration> ConfigurationFields { get; }

        void Setup(SourceConfig? sourceConfig);

        IEnumerable<ValidationError> ValidateConfiguration();

        Stream ReadData();

    }

}
