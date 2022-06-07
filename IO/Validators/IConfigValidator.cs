using CommunAxiom.Commons.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public interface IConfigValidator
    {
        ValidationError Validate(DataSourceConfiguration configuration);
    }
}