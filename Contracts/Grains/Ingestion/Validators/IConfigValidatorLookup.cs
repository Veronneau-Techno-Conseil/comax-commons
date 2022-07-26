using System.Collections.Generic;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IConfigValidatorLookup
    {
        IList<IConfigValidator> ConfigValidators { get; }
    }
}