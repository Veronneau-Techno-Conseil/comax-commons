namespace CommunAxiom.Commons.Ingestion.Validators
{
    public interface IConfigValidatorOptions : IConfigValidatorLookup
    {
        void Add(IConfigValidator fieldValidator);
    }
}