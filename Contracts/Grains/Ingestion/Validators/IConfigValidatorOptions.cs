namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IConfigValidatorOptions : IConfigValidatorLookup
    {
        void Add(IConfigValidator validator);
    }
}