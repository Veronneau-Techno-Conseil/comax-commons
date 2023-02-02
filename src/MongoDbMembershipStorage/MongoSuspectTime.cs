using MongoDB.Bson.Serialization.Attributes;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MongoDbMembershipStorage
{
    public sealed class MongoSuspectTime
    {
        [BsonRequired]
        public string Address { get; set; }

        [BsonRequired]
        public string IAmAliveTime { get; set; }

        public static MongoSuspectTime Create(Tuple<SiloAddress, DateTime> tuple)
        {
            return new MongoSuspectTime { Address = tuple.Item1.ToParsableString(), IAmAliveTime = LogFormatter.PrintDate(tuple.Item2) };
        }

        public Tuple<SiloAddress, DateTime> ToTuple()
        {
            return Tuple.Create(SiloAddress.FromParsableString(Address), LogFormatter.ParseDate(IAmAliveTime));
        }
    }
}
