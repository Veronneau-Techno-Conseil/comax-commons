using Orleans;

namespace Referee.Contracts
{
    public class UpdateRowRequest
    {
        public MembershipEntry Entry { get; set; }
        public string Etag { get; set; }
        public TableVersion TableVersion { get; set; }
    }
}