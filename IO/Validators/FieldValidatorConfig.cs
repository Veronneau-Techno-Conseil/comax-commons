namespace CommunAxiom.Commons.Ingestion.Validators
{
    /// <summary>
    /// 
    /// </summary>
    public class FieldValidatorOptions : IFieldValidatorOptions
    {
        private readonly Dictionary<string, IFieldValidator> fieldValidators;

        public void Add(IFieldValidator fieldValidator)
        {
            fieldValidators.Add(fieldValidator.Tag, fieldValidator);
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