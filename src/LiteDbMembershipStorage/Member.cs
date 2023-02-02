using Newtonsoft.Json;
using Orleans.Runtime;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using SiloAddressClass = Orleans.Runtime.SiloAddress;

namespace Comax.Commons.Orchestrator.LiteDbMembershipStorage
{
    public sealed class Member
    {
        public string Etag { get; set; }

        public string HostName { get; set; }

        public string SiloAddress { get; set; }

        public string SiloName { get; set; }

        public string RoleName { get; set; }

        public string StatusText { get; set; }

        public DateTime IAmAliveTime { get; set; }

        public string StartTime { get; set; }

        public int ProxyPort { get; set; }

        public int UpdateZone { get; set; }

        public int FaultZone { get; set; }

        public int Status { get; set; }

        public List<SuspectTime> SuspectTimes { get; set; }

        public DateTime Timestamp { get; set; }

        public static Member Create(MembershipEntry entry)
        {
            var suspectTimes =
                entry.SuspectTimes?.Select(SuspectTime.FromOrleans).ToList() ?? new List<SuspectTime>();

            return new Member
            {
                Etag = Guid.NewGuid().ToString(),
                FaultZone = entry.FaultZone,
                HostName = entry.HostName,
                IAmAliveTime = entry.IAmAliveTime,
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
                IAmAliveTime = IAmAliveTime,
                ProxyPort = ProxyPort,
                RoleName = RoleName,
                SiloAddress = SiloAddressClass.FromParsableString(SiloAddress),
                SiloName = SiloName,
                Status = (SiloStatus)Status,
                StartTime = LogFormatter.ParseDate(StartTime),
                SuspectTimes = SuspectTimes.Select(x => x.ToOrleans()).ToList(),
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
