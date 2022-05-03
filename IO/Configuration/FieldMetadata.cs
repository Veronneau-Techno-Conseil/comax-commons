using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Configuration
{
    public class FieldMetadata
    {
        /// <summary>
        /// Name of the metadata field
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Defines if field metadata targets a dataset, a datasource or another target
        /// </summary>
        public ConfigurationTarget ConfigurationTarget { get; set; } = ConfigurationTarget.Dataset;
        /// <summary>
        /// Defined what type of field this is
        /// </summary>
        public FieldType FieldType { get; set; }
        /// <summary>
        /// Validator object to use with field to validate either the configuration or the value of the actual data
        /// </summary>
        public IFieldManager Manager { get; set; }
        /// <summary>
        /// Defines is this field must have a value
        /// </summary>
        public bool Mandatory { get; set; } = false;
        /// <summary>
        /// Additinonal parameters to be set during configuration. This should be a json object
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// This represents the actual field name in the object
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// This represents the expected index of the value (should be used for instance on ordered column CSVs and array parsing)
        /// </summary>
        public int? Index { get; set; }
        /// <summary>
        /// Input model to use to define the behavior and options of this field, to be defined during coding
        /// </summary>
        public string Model { get; set; }
    }
}
