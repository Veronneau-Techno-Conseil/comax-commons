using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class FileConfigValidator : IConfigValidator
    {
        public ValidationError Validate(DataSourceConfiguration config)
        {
            if (config.FieldType == FieldType.File)
            {
                return new ValidationError();
            }
            return new ValidationError();
        }
    }
}