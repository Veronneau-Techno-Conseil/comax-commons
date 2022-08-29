using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using Newtonsoft.Json;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class FileConfigValidator : IConfigValidator
    {
        public ValidationError Validate(DataSourceConfiguration configuration)
        {
            if (configuration.FieldType == FieldType.File)
            {
                try
                {
                    var file = JsonConvert.DeserializeObject<FileModel>(configuration.Value);

                    if (file == null) throw new Exception();
                }
                catch
                {
                    return new ValidationError
                    {
                        ErrorCode = "The file type is required to set file name and file path",
                        FieldName = configuration.Name
                    };
                }
            }
            return null;
        }
    }
}