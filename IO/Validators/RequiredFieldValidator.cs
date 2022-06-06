using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class RequiredFieldValidator : IFieldValidator
    {
        public string Tag => "required";

        public string Parameter { get; set; }

        public ValidationError Validate(DataSourceConfiguration configuration, JToken field)
        {
            if (configuration == null)
                return null;

            if (field == null || field.HasValues)
            {
                return new ValidationError { FieldName = configuration.Name, ErrorCode = "required" };
            }

            return null;
        }

    }
}
