using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    public interface IDataSourceReader
    {
        IngestionType IngestionType { get; }

        IEnumerable<DataSourceConfiguration> ConfigurationFields { get; }

        IEnumerable<FieldMetaData> DataDescription { get; }

        void Setup(SourceConfig? sourceConfig);

        IEnumerable<ValidationError> ValidateConfiguration();

        Stream ReadData();

    }

}
