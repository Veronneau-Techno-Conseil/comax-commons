using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using Newtonsoft.Json;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    [DataSourceType(DataSourceType.File)]
    public class TextDataSourceReader : IDataSourceReader
    {
        private readonly IFieldValidatorLookup _fieldValidatorLookup;

        public TextDataSourceReader(IFieldValidatorLookup fieldValidatorLookup)
        {
            _fieldValidatorLookup = fieldValidatorLookup;
        }

        public TextDataSourceReader()
        {

        }

        public IngestionType IngestionType => IngestionType.JSON;

        private IEnumerable<DataSourceConfiguration> _dataSourceConfigurations;

        public IEnumerable<DataSourceConfiguration> ConfigurationFields => _dataSourceConfigurations;

        public IEnumerable<FieldMetaData> DataDescription => throw new NotImplementedException();

        public Stream ReadData()
        {
            var dataSourceConfiguration = _dataSourceConfigurations.FirstOrDefault();

            if (dataSourceConfiguration == null)
            {
                throw new Exception("error");
            }

            var file = JsonConvert.DeserializeObject<Configuration.File>(dataSourceConfiguration.Value);

            using var fileStream = new FileStream(Path.Combine(file.Path, file.Name), FileMode.Open, FileAccess.Read);

            return fileStream;

        }

        public void Setup(SourceConfig sourceConfig = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ValidationError> ValidateConfiguration()
        {
            foreach (var configuration in ConfigurationFields)
            {
                foreach (var validate in configuration.Validators)
                {
                    var validator = _fieldValidatorLookup.Get(validate.Tag);
                    if (validator != null)
                    {
                        yield return validator.Validate(configuration);
                    }
                }
            }
        }
    }
}
