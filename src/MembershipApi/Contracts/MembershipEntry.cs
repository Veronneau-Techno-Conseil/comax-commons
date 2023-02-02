using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommunAxiom.Commons.CommonsShared.MembershipApi.Contracts
{
    public class MembershipEntry
    {
        /// <summary>
        /// The silo unique identity (ip:port:epoch). Used mainly by the Membership Protocol.
        /// </summary>
        public SiloAddress SiloAddress { get; set; }

        /// <summary>
        /// The silo status. Managed by the Membership Protocol.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// The list of silos that suspect this silo. Managed by the Membership Protocol.
        /// </summary>
        public List<SuspectTimeTuple> SuspectTimes { get; set; }

        /// <summary>
        /// Silo to clients TCP port. Set on silo startup.
        /// </summary>    
        public int ProxyPort { get; set; }

        /// <summary>
        /// The DNS host name of the silo. Equals to Dns.GetHostName(). Set on silo startup.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// the name of the specific silo instance within a cluster. 
        /// If running in Azure - the name of this role instance. Set on silo startup.
        /// </summary>
        public string SiloName { get; set; }

        public string RoleName { get; set; } // Optional - only for Azure role  
        public int UpdateZone { get; set; }  // Optional - only for Azure role
        public int FaultZone { get; set; }   // Optional - only for Azure role

        /// <summary>
        /// Time this silo was started. For diagnostics and troubleshooting only.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// the last time this silo reported that it is alive. For diagnostics and troubleshooting only.
        /// </summary>
        public DateTime IAmAliveTime { get; set; }


        public static MembershipEntry Parse(Orleans.MembershipEntry entry)
        {
            return new MembershipEntry
            {
                FaultZone = entry.FaultZone,
                HostName = entry.HostName,
                IAmAliveTime = entry.IAmAliveTime,
                ProxyPort = entry.ProxyPort,
                RoleName = entry.RoleName,
                SiloAddress = SiloAddress.Parse(entry.SiloAddress),
                SiloName = entry.SiloName,
                StartTime = entry.StartTime,
                Status = (int)entry.Status,
                SuspectTimes = entry.SuspectTimes.Select(x => new SuspectTimeTuple
                {
                    Item1 = SiloAddress.Parse(x.Item1),
                    Item2 = x.Item2
                }).ToList(),
                UpdateZone = entry.UpdateZone
            };
        }

        public Orleans.MembershipEntry ToOrleans()
        {
            return new Orleans.MembershipEntry
            {
                FaultZone = this.FaultZone,
                HostName = this.HostName,
                IAmAliveTime = this.IAmAliveTime,
                ProxyPort = this.ProxyPort,
                RoleName = this.RoleName,
                SiloAddress = this.SiloAddress.ToOrleans(),
                SiloName = this.SiloName,
                StartTime = this.StartTime,
                Status = (Orleans.Runtime.SiloStatus)(int)this.Status,
                SuspectTimes = this.SuspectTimes.Select(x => new Tuple<Orleans.Runtime.SiloAddress, DateTime>(
                    x.Item1.ToOrleans(),
                    x.Item2
                    )
                ).ToList(),
                UpdateZone = this.UpdateZone
            };
        }
    }
}
