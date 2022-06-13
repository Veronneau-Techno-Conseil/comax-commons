using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using Newtonsoft.Json;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    [DataSourceType(DataSourceType.File)]
    public class TextDataSourceReader : IDataSourceReader
    {
        private readonly IConfigValidatorLookup _configValidatorLookup;

        private IList<DataSourceConfiguration> _dataSourceConfigurations;
        public IList<DataSourceConfiguration> ConfigurationFields => _dataSourceConfigurations;

        public IngestorType IngestorType => IngestorType.JSON;
        public IList<FieldMetaData> DataDescription => throw new NotImplementedException();


        public TextDataSourceReader(IConfigValidatorLookup configValidatorLookup)
        {
            _configValidatorLookup = configValidatorLookup;
        }


        public Stream ReadData()
        {
            if (_dataSourceConfigurations == null)
            {
                throw new NullReferenceException("There is no data source configuration!");
            }

            var dataSourceConfiguration = _dataSourceConfigurations.FirstOrDefault();

            var file = JsonConvert.DeserializeObject<Configuration.File>(dataSourceConfiguration.Value);

            if (file == null)
            {
                throw new NullReferenceException("File wasn't able to deserialized!");
            }

            return new FileStream(Path.Combine(file.Path, file.Name), FileMode.Open, FileAccess.Read);

        }

        public void Setup(SourceConfig? sourceConfig)
        {
            List<DataSourceConfiguration> list = new List<DataSourceConfiguration>();
            if (_dataSourceConfigurations == null)
            {
                list.AddRange(sourceConfig.Configurations.Select(x => x.Value));
            }
            _dataSourceConfigurations = list.ToArray();
        }

        public IEnumerable<ValidationError> ValidateConfiguration()
        {
            foreach (var configuration in _dataSourceConfigurations)
            {
                foreach (var validator in _configValidatorLookup.ConfigValidators)
                {
                    yield return validator.Validate(configuration);
                }
            }

        }

        public void ClearCofigurations()
        {
            _dataSourceConfigurations = null;
        }
    }
}
