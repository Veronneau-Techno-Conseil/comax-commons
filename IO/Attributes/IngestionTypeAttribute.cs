using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IngestionTypeAttribute : Attribute
    {
        public IngestionType IngestionType { get; private set; }

        public IngestionTypeAttribute(IngestionType ingestionType)
        {
            IngestionType = ingestionType;
        }
    }

}

