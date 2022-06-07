using CommunAxiom.Commons.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Configuration
{
    public class FieldMetaData
    {
        public string Name { get; set; }

        public FieldType FieldType { get; set; }

        public IList<IFieldValidator> Validators { get; set; }

        public int? Index { get; set; }
    }
}
