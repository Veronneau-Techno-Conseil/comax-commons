using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Comax.Commons.Orchestrator.MembershipProvider.Models;

namespace Comax.Commons.Orchestrator.MongoDbMembershipStorage
{
    public class Cluster
    {
        public Cluster() 
        {
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string DeploymentId { get; set; }

        [BsonRequired]
        public int Version { get; set; }

        [BsonRequired]
        public string VersionEtag { get; set; }

        [BsonRequired]
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
