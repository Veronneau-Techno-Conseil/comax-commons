using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using System.Collections.Generic;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration
{
    public class FieldMetaData
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }

        public FieldType FieldType { get; set; }

        public IList<IFieldValidator> Validators { get; set; }

        public int? Index { get; set; }
    }
}
