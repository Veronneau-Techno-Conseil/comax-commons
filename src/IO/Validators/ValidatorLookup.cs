using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class ValidatorLookup : IFieldValidatorOptions, IConfigValidatorOptions
    {
        private readonly Dictionary<FieldType, IFieldValidator> _fieldValidators;
        private readonly IDictionary<ConfigurationFieldType, IList<IConfigValidator>> _configValidators;
        
        public IDictionary<string, IConfigValidator> CustomConfigValidators { get; } 
        public IDictionary<string, IFieldValidator> CustomFieldValidators { get; }

        public ValidatorLookup()
        {
            _fieldValidators = new Dictionary<FieldType, IFieldValidator>();
            _configValidators = new Dictionary<ConfigurationFieldType, IList<IConfigValidator>>();
            CustomConfigValidators = new Dictionary<string, IConfigValidator>();
            CustomFieldValidators = new Dictionary<string, IFieldValidator>();
        }

        public void Add(FieldType type, IFieldValidator validator)
        {
            _fieldValidators.Add(type, validator);
        }

        public void Add(string key, IFieldValidator validator)
        {
            CustomFieldValidators.Add(key, validator);
        }

        public void Add(string key, IConfigValidator validator)
        {
            CustomConfigValidators.Add(key, validator);
        }
        
        public void Add(ConfigurationFieldType type, IConfigValidator validator)
        {
            if (_configValidators.Count > 0)
            {
                var selectedValidator = _configValidators[type];
                selectedValidator.Add(validator);
            }
            else
            {
                _configValidators.Add(type, new List<IConfigValidator> { validator });
            }
        }

        public IList<IFieldValidator> Get(FieldType fieldType, params string[] additionalValidators)
        {
            var validators = new List<IFieldValidator>();

            foreach (var validator in additionalValidators)
            {
                validators.Add(CustomFieldValidators[validator]);
            }

            if (_fieldValidators.ContainsKey(fieldType))
            {
                validators.Add(_fieldValidators[fieldType]);
            }

            return validators;
        }

        public IList<IConfigValidator> Get(ConfigurationFieldType configurationFieldType, params string[] additionalValidators)
        {
            var validators = new List<IConfigValidator>();

            foreach (var validator in additionalValidators)
            {
                validators.Add(CustomConfigValidators[validator]);
            }

            if (_configValidators.ContainsKey(configurationFieldType))
            {
                validators.AddRange(_configValidators[configurationFieldType]);
            }

            return validators;
        }
    }
}