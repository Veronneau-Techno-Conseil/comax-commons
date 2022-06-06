using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.DataSource
{
    /// <summary>
    /// 
    /// </summary>
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

        public IEnumerable<DataSourceConfiguration> ConfigurationFields => throw new NotImplementedException();

        public IEnumerable<FieldMetaData> DataDescription => throw new NotImplementedException();

        public Stream ReadData()
        {
            throw new NotImplementedException();
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
                        yield return validator.Validate(configuration, null);
                    }
                }
            }
        }
    }
}
