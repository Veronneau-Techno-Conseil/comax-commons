using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using System.Collections.Generic;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration
{
    public class DataSourceConfiguration
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Hint { get; set; }

        public ConfigurationFieldType FieldType { get; set; }

        public IList<IFieldValidator> Validators { get; set; }

        public string Parameter { get; set; }

        public int? Index { get; set; }

        public string Value { get; set; }
    }

}

