using Orleans;

namespace Referee.Contracts
{
    public class UpdateRowRequest
    {
        public MembershipEntry Entry { get; internal set; }
        public string Etag { get; internal set; }
        public TableVersion TableVersion { get; internal set; }
    }
}