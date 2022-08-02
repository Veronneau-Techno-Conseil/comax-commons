using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class BooleanFieldValidator : IFieldValidator
    {
        public string Tag => "boolean-field-type";

        public string Parameter { get; set; }

        public ValidationError Validate(FieldMetaData field, JObject row)
        {
            if (field == null) return null;

            var value = row[field.FieldName];

            if (row != null && (value == null || value.Type != JTokenType.Boolean))
            {
                return new ValidationError { FieldName = field.FieldName, ErrorCode = "This field has been indicated to be number!" };
            }

            return null;
        }
    }
}
