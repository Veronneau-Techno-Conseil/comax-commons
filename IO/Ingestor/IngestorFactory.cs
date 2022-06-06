using System.Reflection;
using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Injestor;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public class IngestorFactory : IIngestionFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public IIngestor Create(IngestionType ingestionType)
        {
            var type = Assembly.GetAssembly(typeof(IngestorFactory)).GetTypes()
                .FirstOrDefault(type => Attribute.IsDefined(type, typeof(DataSourceTypeAttribute)) &&
                                type.GetCustomAttribute<IngestionTypeAttribute>().IngestionType == ingestionType);

            if (type == null)
            {
                throw new ArgumentException($"No IngestionType with name {ingestionType} could be found");
            }

            var dataSourceReader = (IIngestor)_serviceProvider.GetService(type);

            if (dataSourceReader == null)
            {
                throw new ArgumentException($"No IngestionType with name {ingestionType} could be found");
            }

            return dataSourceReader;
        }
    }
}

