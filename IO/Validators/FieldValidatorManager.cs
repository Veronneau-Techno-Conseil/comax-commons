namespace CommunAxiom.Commons.Ingestion.Validators
{
    /// <summary>
    /// .
    /// </summary>
    public class FieldValidatorManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public void Configure(Action<IFieldValidatorOptions> options)
        {
            options(new FieldValidatorOptions());
        }
    }
}