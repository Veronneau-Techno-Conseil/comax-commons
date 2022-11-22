using System.Collections.Generic;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IFieldValidatorLookup
    {
        IList<IFieldValidator> Get(FieldType configurationFieldType,
            params string[] additionalValidators);
    }

}

