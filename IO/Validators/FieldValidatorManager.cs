namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class ValidatorManager
    {
        public void Configure(Action<IFieldValidatorOptions> options)
        {
            options(new ValidatorLookup());
        }


        public void Configure(Action<IConfigValidatorOptions> options)
        {
            options(new ValidatorLookup());
        }
    }
}