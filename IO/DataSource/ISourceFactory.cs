using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    public interface ISourceFactory
    {
        IDataSourceReader Create(DataSourceType sourceType);
    }
}

