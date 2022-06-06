using CommunAxiom.Commons.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class DataSourceConfiguration
    {
        /// <summary>
        /// Name of the metadata field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines if field metadata targets a dataset, a datasource or another target
        /// </summary>
        // public ConfigurationTarget ConfigurationTarget { get; set; } = ConfigurationTarget.Dataset;
        /// <summary>
        /// Defined what type of field this is
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// Validator object to use with field to validate either the configuration or the value of the actual data
        /// </summary>
        public IList<IFieldValidator> Validators { get; set; }

        /// <summary>
        /// Defines is this field must have a value
        /// </summary>
        //public bool IsRequired { get; set; } = false;
        /// <summary>
        /// Additinonal parameters to be set during configuration. This should be a json object
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// This represents the actual field name in the object
        /// </summary>
        // public string FieldName { get; set; }
        /// <summary>
        /// This represents the expected index of the value (should be used for instance on ordered column CSVs and array parsing)
        /// </summary>
        public int? Index { get; set; }
    }

}

