using Orleans;

namespace Referee.Contracts
{
    public class InsertRowRequest
    {
        public MembershipEntry Entry { get; internal set; }
        public TableVersion TableVersion { get; internal set; }
    }
}