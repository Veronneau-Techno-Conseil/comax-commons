using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class CreateConfigRequest
    {
        public string Id { get; set; }
        public SourceConfig Config { get; set; }
    }
}