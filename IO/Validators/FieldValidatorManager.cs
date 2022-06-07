namespace CommunAxiom.Commons.Ingestion.Validators
{
    public class ValidatorManager
    {
        public void Configure(Action<IFieldValidatorOptions> options)
        {
            options(new ValidatorOptions());
        }


        public void Configure(Action<IConfigValidatorOptions> options)
        {
            options(new ValidatorOptions());
        }
    }
}