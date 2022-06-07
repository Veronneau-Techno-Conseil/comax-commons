namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class ValidatorOptions : IFieldValidatorOptions, IConfigValidatorOptions
    {
        private readonly Dictionary<string, IFieldValidator> fieldValidators;
        private readonly IList<IConfigValidator> configValidators;

        public IList<IConfigValidator> Validators => configValidators;

        public void Add(IFieldValidator fieldValidator)
        {
            fieldValidators.Add(fieldValidator.Tag, fieldValidator);
        }

        public void Add(IConfigValidator configValidator)
        {
            configValidators.Add(configValidator);
        }

        public IFieldValidator? Get(string tag)
        {
            return fieldValidators[tag] ?? null;
        }

        public bool TryGet(string tag, out IFieldValidator validator)
        {
            return fieldValidators.TryGetValue(tag, out validator);
        }
    }
}