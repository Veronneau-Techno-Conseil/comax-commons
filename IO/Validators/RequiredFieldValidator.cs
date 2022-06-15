using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class RequiredFieldValidator : IFieldValidator
    {
        public string Tag => "required-field";

        public string Parameter { get; set; }

        public ValidationError Validate(FieldMetaData fieldMetaData, JObject obj)
        {
            if (fieldMetaData == null)
                return null;

            if (obj != null && (obj[fieldMetaData.Name] == null || obj[fieldMetaData.Name].HasValues))
            {
                return new ValidationError { FieldName = fieldMetaData.Name, ErrorCode = "This field is required!" };
            }

            return null;
        }

    }
}
