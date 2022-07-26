using CommunAxiom.Commons.Client.Contracts.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class ValidatorManager
    {
        private ValidatorLookup _validatorLookup;

        public ValidatorManager()
        {
            _validatorLookup = new ValidatorLookup();
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