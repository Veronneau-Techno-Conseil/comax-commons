using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class FileConfigValidator : IConfigValidator
    {
        public ValidationError Validate(DataSourceConfiguration configuration)
        {
            if (configuration.FieldType == ConfigurationFieldType.File)
            {
                try
                {
                    if (string.IsNullOrEmpty(configuration.Value))
                    {
                        throw new Exception();
                    }

                    var fileInfo = new FileInfo(configuration.Value);

                    if (fileInfo.Exists && fileInfo.Length > 0)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    return new ValidationError
                    {
                        ErrorCode = "File is not exists or file length is zero.",
                        FieldName = configuration.Name
                    };
                }
            }
            return null;
        }
    }

    public class RequiredConfigValidator : IConfigValidator
    {
        public ValidationError Validate(DataSourceConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}