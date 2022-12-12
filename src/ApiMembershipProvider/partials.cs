using Orleans.Runtime;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.ApiMembershipProvider.ApiRef
{
    public partial class SiloAddress
    {
        public Uri GetUri(int? port = null)
        {
            return Orleans.Runtime.SiloAddress.New(new System.Net.IPEndPoint(this.Address, port != null ? port.Value : Port), this.Gen).ToGatewayUri();
        }

        public static SiloAddress Parse(Orleans.Runtime.SiloAddress entry)
        {
            return new ApiRef.SiloAddress
            {
                Address = entry.Endpoint.Address.Address,
                Port = entry.Endpoint.Port,
                Gen = entry.Generation
            };
        }
        public Orleans.Runtime.SiloAddress ToOrleans()
        {
            return Orleans.Runtime.SiloAddress.New(new System.Net.IPEndPoint(this.Address, this.Port), this.Gen);
        }
    }

    public partial class MembershipEntry
    {
        public static MembershipEntry Parse(Orleans.MembershipEntry entry)
        {
            return new ApiRef.MembershipEntry
            {
                FaultZone = entry.FaultZone,
                HostName = entry.HostName ?? "",
                IAmAliveTime = entry.IAmAliveTime,
                ProxyPort = entry.ProxyPort,
                RoleName = entry.RoleName ?? "",
                SiloAddress = ApiRef.SiloAddress.Parse(entry.SiloAddress),
                SiloName = entry.SiloName ?? "",
                StartTime = entry.StartTime,
                Status = (int)entry.Status,
                SuspectTimes = (entry.SuspectTimes ?? new List<Tuple<Orleans.Runtime.SiloAddress, DateTime>>()).Select(x => new ApiRef.SuspectTimeTuple
                {
                    Item1 = ApiRef.SiloAddress.Parse(x.Item1),
                    Item2 = new DateTimeOffset(x.Item2)
                }).ToList(),
                UpdateZone = entry.UpdateZone
            };
        }

        public  Orleans.MembershipEntry ToOrleans()
        {
            return new Orleans.MembershipEntry
            {
                FaultZone = this.FaultZone,
                HostName = this.HostName,
                IAmAliveTime = this.IAmAliveTime.DateTime,
                ProxyPort = this.ProxyPort,
                RoleName = this.RoleName,
                SiloAddress = this.SiloAddress.ToOrleans(),
                SiloName = this.SiloName,
                StartTime = this.StartTime.DateTime,
                Status = (Orleans.Runtime.SiloStatus)(int)this.Status,
                SuspectTimes = this.SuspectTimes.Select(x => new Tuple<Orleans.Runtime.SiloAddress, DateTime>(
                    x.Item1.ToOrleans(),
                    x.Item2.DateTime
                    )
                ).ToList(),
                UpdateZone = this.UpdateZone
            };
        }
    }

    public partial class TableVersion
    {
        public static TableVersion Parse(Orleans.TableVersion tableVersion)
        {
            return new TableVersion
            {
                Version = tableVersion.Version,
                VersionEtag = tableVersion.VersionEtag
            };
        }

        public Orleans.TableVersion ToOrleans()
        {
            return new Orleans.TableVersion(this.Version, this.VersionEtag);
        }
    }

    public partial class MembershipTableData
    {
        public Orleans.MembershipTableData ToOrleans()
        {
            var members = this.Members.Select(x => new Tuple<Orleans.MembershipEntry, string>(x.Item1.ToOrleans(), x.Item2)).ToList();
            var v = this.Version.ToOrleans();
            return new Orleans.MembershipTableData(members, v);
        }
    }
}
