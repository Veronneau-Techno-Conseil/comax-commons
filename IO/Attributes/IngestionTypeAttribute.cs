using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IngestionTypeAttribute : Attribute
    {
        public IngestorType IngestorType { get; private set; }

        public IngestionTypeAttribute(IngestorType ingestorType)
        {
            IngestorType = ingestorType;
        }
    }
}

