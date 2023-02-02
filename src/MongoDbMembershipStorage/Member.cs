using Newtonsoft.Json;
using Orleans.Runtime;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SiloAddressClass = Orleans.Runtime.SiloAddress;

namespace Comax.Commons.Orchestrator.MongoDbMembershipStorage
{
    public sealed class Member
    {
        [BsonRequired]
        public string Etag { get; set; }

        [BsonRequired]
        public string HostName { get; set; }

        [BsonRequired]
        public string SiloAddress { get; set; }

        [BsonRequired]
        public string SiloName { get; set; }

        [BsonRequired]
        public string RoleName { get; set; }

        [BsonIgnoreIfNull]
        public string StatusText { get; set; }

        [BsonRequired]
        public string IAmAliveTime { get; set; }

        [BsonRequired]
        public string StartTime { get; set; }

        [BsonRequired]
        public int ProxyPort { get; set; }

        [BsonRequired]
        public int UpdateZone { get; set; }

        [BsonRequired]
        public int FaultZone { get; set; }

        [BsonRequired]
        public int Status { get; set; }

        [BsonRequired]
        public List<MongoSuspectTime> SuspectTimes { get; set; }

        [BsonIgnoreIfDefault]
        public DateTime Timestamp { get; set; }

        public static Member Create(MembershipEntry entry)
        {
            var suspectTimes =
                entry.SuspectTimes?.Select(MongoSuspectTime.Create).ToList() ?? new List<MongoSuspectTime>();

            return new Member
            {
                Etag = Guid.NewGuid().ToString(),
                FaultZone = entry.FaultZone,
                HostName = entry.HostName,
                IAmAliveTime = LogFormatter.PrintDate(entry.IAmAliveTime),
                ProxyPort = entry.ProxyPort,
                RoleName = entry.RoleName,
                SiloAddress = entry.SiloAddress.ToParsableString(),
                SiloName = entry.SiloName,
                Status = (int)entry.Status,
                StatusText = entry.Status.ToString(),
                StartTime = LogFormatter.PrintDate(entry.StartTime),
                SuspectTimes = suspectTimes,
                Timestamp = entry.IAmAliveTime,
                UpdateZone = entry.UpdateZone
            };
        }

        public MembershipEntry ToEntry()
        {
            return new MembershipEntry
            {
                FaultZone = FaultZone,
                HostName = HostName,
                IAmAliveTime = LogFormatter.ParseDate(IAmAliveTime),
                ProxyPort = ProxyPort,
                RoleName = RoleName,
                SiloAddress = SiloAddressClass.FromParsableString(SiloAddress),
                SiloName = SiloName,
                Status = (SiloStatus)Status,
                StartTime = LogFormatter.ParseDate(StartTime),
                SuspectTimes = SuspectTimes.Select(x => x.ToTuple()).ToList(),
                UpdateZone = UpdateZone
            };
        }

        public Uri ToGatewayUri()
        {
            var siloAddress = SiloAddressClass.FromParsableString(SiloAddress);

            return SiloAddressClass.New(new IPEndPoint(siloAddress.Endpoint.Address, ProxyPort), siloAddress.Generation).ToGatewayUri();
        }
    }
}
