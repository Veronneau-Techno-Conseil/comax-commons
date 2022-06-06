using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class RequiredFieldValidator : IFieldValidator
    {
        public string Tag => "required";

        public string Parameter { get; set; }

        public ValidationError Validate(DataSourceConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        //public ValidationError Validate(DataSourceConfiguration configuration, JToken field)
        //{
        //    if (configuration == null)
        //        return null;

        //    if (field == null || field.HasValues)
        //    {
        //        return new ValidationError { FieldName = configuration.Name, ErrorCode = "required" };
        //    }

        //    return null;
        //}

    }
}
