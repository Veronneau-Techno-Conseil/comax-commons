using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.LiteDbMembershipStorage
{
    public sealed class SuspectTime
    {
        public string Address { get; set; }

        public string MarkedTime { get; set; }

        public static SuspectTime FromOrleans(Tuple<SiloAddress, DateTime> tuple)
        {
            return new SuspectTime { Address = tuple.Item1.ToParsableString(), MarkedTime = LogFormatter.PrintDate(tuple.Item2) };
        }

        public Tuple<SiloAddress, DateTime> ToOrleans()
        {
            return Tuple.Create(SiloAddress.FromParsableString(Address), LogFormatter.ParseDate(MarkedTime));
        }
    }
}
