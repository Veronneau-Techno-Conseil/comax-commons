using System.Reflection;
using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    public sealed class DataSourceFactory : IDataSourceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DataSourceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDataSourceReader Create(DataSourceType sourceType)
        {
            var type = Assembly.GetAssembly(typeof(DataSourceFactory)).GetTypes()
                .FirstOrDefault(type => Attribute.IsDefined(type, typeof(DataSourceTypeAttribute)) &&
                                type.GetCustomAttribute<DataSourceTypeAttribute>().DataSourceType == sourceType);

            if (type == null)
            {
                throw new ArgumentException($"No DataSourceReader type with name {Enum.GetName(sourceType)} could be found");
            }

            var reader = (IDataSourceReader)_serviceProvider.GetService(type);

            if (reader == null)
            {
                throw new NullReferenceException($"No DataSourceReader resolved with type {type.FullName}");
            }

            return reader;
        }
    }
}

