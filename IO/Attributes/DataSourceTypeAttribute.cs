using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataSourceTypeAttribute : Attribute
    {
        public DataSourceType DataSourceType { get; private set; }

        public DataSourceTypeAttribute(DataSourceType sourceType)
        {
            DataSourceType = sourceType;
        }

    }
}

