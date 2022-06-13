using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class RequiredFieldValidator : IFieldValidator
    {
        public string Tag => "required-field";

        public string Parameter { get; set; }

        public ValidationError Validate(DataSourceConfiguration configuration, JObject obj)
        {
            if (configuration == null)
                return null;

            if (obj != null && (obj[configuration.Name] == null || obj[configuration.Name].HasValues))
            {
                return new ValidationError { FieldName = configuration.Name, ErrorCode = "This field is required!" };
            }

            return null;
        }

    }
}
