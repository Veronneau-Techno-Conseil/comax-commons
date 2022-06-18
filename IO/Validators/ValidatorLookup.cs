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

        public void Add(IFieldValidator validator)
        {
            _fieldValidators.Add(validator.Tag, validator);
        }

        public void Add(IConfigValidator validator)
        {
            _configValidators.Add(validator);
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