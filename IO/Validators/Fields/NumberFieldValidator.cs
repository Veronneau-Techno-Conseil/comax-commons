using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class NumberFieldValidator : IFieldValidator
    {
        public string Tag => "number-field-type";

        public string Parameter { get; set; }

        public ValidationError Validate(FieldMetaData field, JObject row)
        {
            if (field == null) return null;

            var value = row[field.FieldName];

            if (row != null && (value == null || value.Type != JTokenType.Integer))
            {
                return new ValidationError { FieldName = field.FieldName, ErrorCode = "This field has been indicated to be number!" };
            }

            return null;
        }
    }
}
