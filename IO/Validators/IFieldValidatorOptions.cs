namespace CommunAxiom.Commons.Ingestion.Validators
{
    public interface IFieldValidatorOptions : IFieldValidatorLookup
    {
        void Add(IFieldValidator validator);
    }
}