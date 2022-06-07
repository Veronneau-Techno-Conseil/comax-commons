namespace CommunAxiom.Commons.Ingestion.Validators
{
    public interface IConfigValidatorLookup
    {
        IList<IConfigValidator> Validators { get; }
    }
}