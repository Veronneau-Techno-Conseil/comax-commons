using System.Security.Cryptography;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;
using CommunAxiom.Commons.Ingestion.Extentions;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    public class TextDataSourceReader : IDataSourceReader
    {
        private readonly IConfigValidatorLookup _configValidatorLookup;

        private Dictionary<string, DataSourceConfiguration> _configurations;
        public Dictionary<string, DataSourceConfiguration> Configurations => _configurations;

        public IngestorType IngestorType
        {
            get
            {
                if (_configurations == null && _configurations["ContentType"] == null)
                {
                    throw new NullReferenceException("Configurations is NULL!");
                }

                return _configurations["ContentType"].Value.ToEnum<IngestorType>();
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

            var dataSourceConfiguration = _configurations.FirstOrDefault(x => x.Key == "FilePath").Value;

            return new FileStream(dataSourceConfiguration.Value, FileMode.Open, FileAccess.Read);
        }

        public byte[] CalculateHash()
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(ReadData());
        }

        /*
         *
        public Stream ReadSampleFile()
        {
            if (_configurations == null)
            {
                throw new NullReferenceException("There is no data source configuration!");
            }
        
            var dataSourceConfiguration = _configurations.FirstOrDefault(x => x.Key == "SampleFile").Value;
            
            if (dataSourceConfiguration.Value == null)
            {
                throw new NullReferenceException("DataSource configuration file cannot be null.");
            }
        
            return new MemoryStream(Encoding.UTF8.GetBytes(dataSourceConfiguration.Value));
        }
        *
        */

        public void Setup(SourceConfig? sourceConfig = null)
        {
            var configurations = new Dictionary<string, DataSourceConfiguration>();

            configurations.Add("ContentType", new DataSourceConfiguration
            {
                Name = "ContentType",
                DisplayName = "Content Type",
                FieldType = ConfigurationFieldType.Lookup,
                Value = "json",
                Parameter = "['json', 'csv']",
                IsReadonly = true,
                Validators = _configValidatorLookup.Get(ConfigurationFieldType.Lookup)
            });

            configurations.Add("SampleFile", new DataSourceConfiguration
            {
                Name = "SampleFile",
                DisplayName = "Sample File",
                FieldType = ConfigurationFieldType.File,
                IsReadonly = true,
                Validators = _configValidatorLookup.Get(ConfigurationFieldType.File, "required")
            });

            configurations.Add("FilePath", new DataSourceConfiguration
            {
                Name = "FilePath",
                DisplayName = "File Path",
                FieldType = ConfigurationFieldType.Text,
                IsReadonly = false,
                Validators = _configValidatorLookup.Get(ConfigurationFieldType.Text, "required")
            });


            if (sourceConfig != null)
            {
                foreach (var item in configurations.Keys)
                {
                    if (sourceConfig.Configurations != null && sourceConfig.Configurations.ContainsKey(item))
                    {
                        configurations[item].Value = sourceConfig.Configurations[item].Value;
                        configurations[item].DisplayValue = sourceConfig.Configurations[item].DisplayValue;
                    }
                }
            }

            _configurations = configurations;
        }

        public IEnumerable<ValidationError> ValidateConfiguration()
        {
            foreach (var configuration in _configurations.Values)
            {
                if (configuration == null || configuration.Validators == null)
                    continue;
                foreach (var validator in configuration.Validators)
                {
                    yield return validator.Validate(configuration);
                }
            }
        }
        
    }
}