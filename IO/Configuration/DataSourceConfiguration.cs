using CommunAxiom.Commons.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Configuration
{
    public class DataSourceConfiguration
    {
        public string Name { get; set; }

        public FieldType FieldType { get; set; }

        public IList<IFieldValidator> Validators { get; set; }

        public string Parameter { get; set; }

        public int? Index { get; set; }

        public string Value { get; set; }
    }

}

