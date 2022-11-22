using System.Collections.Generic;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IConfigValidatorLookup
    {
        IList<IConfigValidator> Get(ConfigurationFieldType configurationFieldType,
            params string[] additionalValidators);

    }
}