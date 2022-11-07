using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Extentions;
using Newtonsoft.Json;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    [DataSourceType(DataSourceType.FILE)]
    public class TextDataSourceReader : IDataSourceReader
    {
        private readonly IConfigValidatorLookup _configValidatorLookup;

        private Dictionary<string, DataSourceConfiguration> _configurations;
        public Dictionary<string, DataSourceConfiguration> Configurations => _configurations;

        public IngestorType IngestorType
        {
            get
            {
                if (_configurations == null && _configurations["content-type"] == null)
                {
                    throw new NullReferenceException("Configurations is NULL!");
                }

                return _configurations["content-type"].Value.ToEnum<IngestorType>();
            }
        }

        public TextDataSourceReader(IConfigValidatorLookup configValidatorLookup)
        {
            _configValidatorLookup = configValidatorLookup;
        }

        public Stream ReadData()
        {
            if (_configurations == null)
            {
                throw new NullReferenceException("There is no data source configuration!");
            }

            var dataSourceConfiguration = _configurations.FirstOrDefault(x => x.Key == "SampleFile").Value;

            var file = JsonConvert.DeserializeObject<FileModel>(dataSourceConfiguration.Value);

            if (file == null)
            {
                throw new NullReferenceException("File wasn't able to deserialized!");
            }

            return new FileStream(Path.Combine(file.Path, file.Name), FileMode.Open, FileAccess.Read);

        }

        public void Setup(SourceConfig? sourceConfig = null)
        {
            Dictionary<string, DataSourceConfiguration> configurations = new Dictionary<string, DataSourceConfiguration>();

            configurations.Add("content-type", new DataSourceConfiguration
            {
                DisplayName = "Content Type",
                Name = "content-type",
                FieldType = ConfigurationFieldType.Lookup,
                Value = "json",
                Parameter = "['json', 'csv']"
            });

            configurations.Add("SampleFile",
                new DataSourceConfiguration { 
                    DisplayName = "Sample File",
                    Name = "SampleFile",
                    FieldType = ConfigurationFieldType.File 
                });

            configurations.Add("FilePath", new DataSourceConfiguration
            {
                Name = "FilePath",
                DisplayName = "File Path",
                FieldType = ConfigurationFieldType.Text
            });


            if (sourceConfig != null)
            {
                foreach (var item in configurations.Keys)
                {
                    if (sourceConfig.Configurations.ContainsKey(item))
                    {
                        configurations[item] = sourceConfig.Configurations[item];
                    }
                }
            }

            _configurations = configurations;
        }

        public IEnumerable<ValidationError> ValidateConfiguration()
        {
            foreach (var configuration in _configurations.Values)
            {
                foreach (var validator in _configValidatorLookup.ConfigValidators)
                {
                    yield return validator.Validate(configuration);
                }
            }
        }
    }
}
