using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class FileConfigValidator : IConfigValidator
    {
        public ValidationError Validate(DataSourceConfiguration config)
        {
            if (config.FieldType == FieldType.File)
            {
                try
                {
                    var file = JsonConvert.DeserializeObject<Configuration.File>(config.Value);

                    if (file == null) throw new Exception();
                }
                catch
                {
                    return new ValidationError
                    {
                        ErrorCode = "The file type is required to set file name and file path",
                        FieldName = config.Name
                    };
                }
            }
            return null;
        }
    }
}