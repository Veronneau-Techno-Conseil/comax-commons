using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IConfigValidatorOptions : IConfigValidatorLookup
    {
        void Add(ConfigurationFieldType type, IConfigValidator validator);
        void Add(string key, IConfigValidator validator);
    }
}