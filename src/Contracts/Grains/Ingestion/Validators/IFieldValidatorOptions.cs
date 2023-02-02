using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IFieldValidatorOptions : IFieldValidatorLookup
    {
        void Add(FieldType type, IFieldValidator validator);
        void Add(string key, IFieldValidator validator);
    }
}