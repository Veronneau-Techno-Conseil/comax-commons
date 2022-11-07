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

namespace Comax.Commons.Orchestrator.MembershipProvider
{
    public class MembershipBase
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

        public static T Create<T>(MembershipEntry entry) where T : MembershipBase, new()
        {
            var suspectTimes =
                entry.SuspectTimes?.Select(MongoSuspectTime.Create).ToList() ?? new List<MongoSuspectTime>();

            return new T
            {
                Etag = EtagHelper.CreateNew(),
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

    public static class EtagHelper
    {
        public static string CreateNew()
        {
            return Guid.NewGuid().ToString();
        }
    }

    public sealed class DeploymentDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string DeploymentId { get; set; }

        [BsonRequired]
        public int Version { get; set; }

        [BsonRequired]
        public string VersionEtag { get; set; }

        [BsonRequired]
        public Dictionary<string, DeploymentMembership> Members { get; set; }

        public TableVersion ToTableVersion()
        {
            return new TableVersion(Version, VersionEtag);
        }

        public MembershipTableData ToTable()
        {
            return new MembershipTableData(Members.Values.Select(x => Tuple.Create(x.ToEntry(), x.Etag)).ToList(), ToTableVersion());
        }

        public MembershipTableData ToTable(string address)
        {
            return new MembershipTableData(Members.Where(x => x.Key == address).Select(x => Tuple.Create(x.Value.ToEntry(), x.Value.Etag)).ToList(), ToTableVersion());
        }
    }
    public sealed class DeploymentMembership : MembershipBase
    {
    }
}
