using Orleans;

namespace Referee.Contracts
{
    public class MembershipTableData
    {
        public List<MembershipEntryTuple> Members { get; set; }

        public TableVersion Version { get; set; }

        public static MembershipTableData Parse(Orleans.MembershipTableData data)
        {
            return new MembershipTableData
            {
                Members = data.Members.Select(x => new MembershipEntryTuple
                {
                    Item1 = MembershipEntry.Parse(x.Item1),
                    Item2 = x.Item2
                }).ToList(),
                Version = TableVersion.Parse(data.Version)
            };
        }

        public Orleans.MembershipTableData ToOrleans()
        {
            return new Orleans.MembershipTableData(
                this.Members.Select(x => new Tuple<Orleans.MembershipEntry, string>(x.Item1.ToOrleans(), x.Item2)).ToList(), 
                this.Version.ToOrleans()
            );
        }
    }
}
