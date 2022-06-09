using System.Reflection;
using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public class IngestorFactory : IIngestorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public IngestorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IIngestor Create(IngestorType ingestorType)
        {
            var type = Assembly.GetAssembly(typeof(IngestorFactory)).GetTypes()
                .FirstOrDefault(type => Attribute.IsDefined(type, typeof(IngestionTypeAttribute)) &&
                                type.GetCustomAttribute<IngestionTypeAttribute>().IngestorType == ingestorType);

            if (type == null)
            {
                throw new ArgumentException($"No IngestionType with name {ingestorType} could be found");
            }

            var dataSourceReader = (IIngestor)_serviceProvider.GetService(type);

            if (dataSourceReader == null)
            {
                throw new NullReferenceException($"No IngestionType resolved with type { type.FullName }");
            }

            return dataSourceReader;
        }
    }
}

