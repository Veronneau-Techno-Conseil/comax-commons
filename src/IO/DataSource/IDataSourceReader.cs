using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    public interface IDataSourceReader
    {
        IngestorType IngestorType { get; }

        Dictionary<string, DataSourceConfiguration> Configurations { get; }

        void Setup(SourceConfig? sourceConfig);

        IEnumerable<ValidationError> ValidateConfiguration();

        Stream ReadData();

        byte[] CalculateHash();
    }

}
