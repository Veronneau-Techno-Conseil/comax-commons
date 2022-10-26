using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class CreateFieldMetaDataRequest
    {
        public string Id { get; set; }
        public List<FieldMetaData> Fields { get; set; }
    }
}