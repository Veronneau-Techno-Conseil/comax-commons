using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    public interface IDataSourceFactory
    {
        IDataSourceReader Create(DataSourceType sourceType);
    }
}

