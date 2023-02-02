using Orleans;

namespace CommunAxiom.Commons.CommonsShared.MembershipApi.Contracts
{
    public class UpdateRowRequest
    {
        public MembershipEntry Entry { get; set; }
        public string Etag { get; set; }
        public TableVersion TableVersion { get; set; }
    }
}