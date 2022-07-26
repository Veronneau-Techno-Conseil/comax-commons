using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IConfigValidator
    {
        ValidationError Validate(DataSourceConfiguration configuration);
    }
}