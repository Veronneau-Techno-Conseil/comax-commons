using Orleans;

namespace CommunAxiom.Commons.CommonsShared.MembershipApi.Contracts
{
    public class InsertRowRequest
    {
        public MembershipEntry Entry { get; set; }
        public TableVersion TableVersion { get; set; }
    }
}