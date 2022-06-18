using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class TextFieldValidator : IFieldValidator
    {
        public string Tag => "text-field-type";

        public string Parameter { get; set; }

        public ValidationError Validate(FieldMetaData field, JObject row)
        {
            if (field == null) return null;

            var value = row[field.FieldName];

            if (row != null && (value == null || value.Type != JTokenType.String))
            {
                return new ValidationError { FieldName = field.FieldName, ErrorCode = "This field has been indicated to be number!" };
            }

            return null;
        }
    }
}
