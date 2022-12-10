using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using System.Collections.Generic;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration
{
    public class DataSourceConfiguration
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Hint { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsRequired { get; set; }

        public ConfigurationFieldType FieldType { get; set; }

        public IList<IConfigValidator> Validators { get; set; } = new List<IConfigValidator>();

        public string Parameter { get; set; }

        public int? Index { get; set; }

        public string Value { get; set; }
        public string DisplayValue { get; set; }
    }

}

