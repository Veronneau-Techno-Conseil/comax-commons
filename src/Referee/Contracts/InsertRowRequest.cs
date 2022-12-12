using Orleans;

namespace Referee.Contracts
{
    public class InsertRowRequest
    {
        public MembershipEntry Entry { get; set; }
        public TableVersion TableVersion { get; set; }
    }
}