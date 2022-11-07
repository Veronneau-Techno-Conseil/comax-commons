using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    public sealed class DataSourceFactory : IDataSourceFactory
    {
        private readonly Func<DataSourceType, IDataSourceReader> _serviceResolver;

        public DataSourceFactory(Func<DataSourceType, IDataSourceReader> serviceResolver)
        {
            _serviceResolver = serviceResolver;
        }

        public IDataSourceReader Create(DataSourceType sourceType)
        {
            var reader = _serviceResolver(sourceType);

            if (reader == null)
            {
                throw new NullReferenceException("No DataSourceReader resolved!");
            }

            return reader;
        }
    }
}

