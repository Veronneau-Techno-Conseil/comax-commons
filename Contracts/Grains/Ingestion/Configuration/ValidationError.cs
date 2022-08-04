namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration
{
    public class ValidationError
    {
        public string ErrorCode { get; set; }
        // position of row
        public string FieldName { get; set; }
    }
}
