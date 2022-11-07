namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class DashboardItem
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ItemCriticality Criticality { get; set; } //0 info, 1 warning, 2 sucess, 3 critical
        public ItemGroup ItemGroup { get; set; }
    }
}