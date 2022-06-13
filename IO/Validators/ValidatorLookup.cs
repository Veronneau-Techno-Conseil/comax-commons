namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class ValidatorLookup : IFieldValidatorOptions, IConfigValidatorOptions
    {
        private readonly Dictionary<string, IFieldValidator> _fieldValidators;
        private readonly IList<IConfigValidator> _configValidators;

        public IList<IConfigValidator> ConfigValidators => _configValidators;

        public ValidatorLookup()
        {
            _fieldValidators = new Dictionary<string, IFieldValidator>();
            _configValidators = new List<IConfigValidator>();
        }

        public void Add(IFieldValidator fieldValidator)
        {
            _fieldValidators.Add(fieldValidator.Tag, fieldValidator);
        }

        public void Add(IConfigValidator configValidator)
        {
            _configValidators.Add(configValidator);
        }

        public IFieldValidator? Get(string tag)
        {
            return _fieldValidators[tag] ?? null;
        }

        public bool TryGet(string tag, out IFieldValidator validator)
        {
            return _fieldValidators.TryGetValue(tag, out validator);
        }
    }
}