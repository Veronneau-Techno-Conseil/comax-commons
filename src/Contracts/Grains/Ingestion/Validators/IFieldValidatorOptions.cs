namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IFieldValidatorOptions : IFieldValidatorLookup
    {
        void Add(IFieldValidator validator);
    }
}