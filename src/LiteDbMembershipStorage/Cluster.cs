
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Comax.Commons.Orchestrator.MembershipProvider.Models;
using LiteDB;

namespace Comax.Commons.Orchestrator.LiteDbMembershipStorage
{
    public class Cluster
    {
        [BsonId]
        public string DeploymentId { get; set; }

        public int Version { get; set; }

        public string VersionEtag { get; set; }

        public Dictionary<string, Member> Members { get; set; }

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

}
