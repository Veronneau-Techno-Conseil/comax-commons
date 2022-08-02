using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class RequiredFieldValidator : IFieldValidator
    {
        public string Tag => "required-field";
        public string Parameter { get; set; }
        public ValidationError Validate(FieldMetaData field, JObject row)
        {
            if (field == null)
                return null;

            if (row != null && (row[field.FieldName] == null || row[field.FieldName].HasValues))
            {
                return new ValidationError { FieldName = field.FieldName, ErrorCode = "This field is required!" };
            }

            return null;
        }

    }
}
