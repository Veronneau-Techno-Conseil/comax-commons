using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class ValidatorManager
    {
        private readonly ValidatorLookup _validatorLookup;

        public ValidatorManager(ValidatorLookup validatorLookup)
        {
            _validatorLookup = validatorLookup;
        }

        public void ConfigureFields(Action<IFieldValidatorOptions> options)
        {
            options(_validatorLookup);
        }
        
        public void ConfigureConfigs(Action<IConfigValidatorOptions> options)
        {
            options(_validatorLookup);
        }
    }
}