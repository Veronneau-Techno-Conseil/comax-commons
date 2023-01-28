using Comax.Commons.Orchestrator.MembershipProvider.Models;
using LiteDB;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.LiteDbMembershipStorage
{
    public class LiteDbMembershipStorageProvider : IMembershipStorage
    {
        LiteDbConfig _config;
        
        protected LiteDatabase Database
        {
            get
            {
                return Common.GetOrAdd(_config.FileName);
            }
        }

        public LiteDbMembershipStorageProvider(LiteDbConfig config)
        { 
            _config = config;
            
        }
        public Task CleanupDefunctSiloEntries(string deploymentId, DateTimeOffset beforeDate)
        {
            return Task.Run(() =>
            {
                var coll = Database.GetCollection<Cluster>();
                var cluster = coll.FindById(deploymentId);
                var lst = cluster.Members.Where(x => x.Value.IAmAliveTime < beforeDate).ToList();
                foreach (var i in lst)
                {
                    cluster.Members.Remove(i.Key);
                }
                coll.Update(cluster);
            });
        }

        public Task DeleteMembershipTableEntries(string deploymentId)
        {
            return Task.Run(() =>
            {
                var coll = Database.GetCollection<Cluster>();
                var cluster = coll.FindById(deploymentId);
                cluster.Members.Clear();
                coll.Update(cluster);
            });
        }

        public Task<IList<Uri>> GetGateways(string deploymentId)
        {
            return Task.Run(() =>
            {
                var coll = Database.GetCollection<Cluster>();
                var cluster = coll.FindById(deploymentId);
                var lst = cluster.Members.Select(x=>x.Value).ToList();
                var res = new List<Uri>();

                return (IList<Uri>)lst.Where(x => x.Status == (int)SiloStatus.Active && x.ProxyPort > 0).Select(x => x.ToGatewayUri()).ToList();
            });
        }

        private Cluster GetOrAdd(string deploymentId)
        {
            var coll = Database.GetCollection<Cluster>();
            var cluster = coll.FindById(deploymentId);
            if(cluster == null)
            {
                cluster = new Cluster()
                {
                    DeploymentId = deploymentId,
                    Version = 0,
                    VersionEtag = Guid.NewGuid().ToString(),
                    Members = new Dictionary<string, Member>()
                };
                coll.Upsert(cluster);
            }
            return cluster;
        }

        public Task<MembershipTableData> ReadAll(string deploymentId)
        {
            return Task.Run(() =>
            {
                var cluster = GetOrAdd(deploymentId);
                return cluster?.ToTable();
            });
        }

        public Task<MembershipTableData> ReadRow(string deploymentId, SiloAddress address)
        {
            return Task.Run(() =>
            {
                var cluster = GetOrAdd(deploymentId);
                return cluster?.ToTable(BuildKey(address));
            });
        }

        public Task UpdateIAmAlive(string deploymentId, SiloAddress address, DateTime iAmAliveTime)
        {
            return Task.Run(() =>
            {
                var coll = Database.GetCollection<Cluster>();
                var cluster = coll.FindById(deploymentId);
                var entry = cluster.Members[BuildKey(address)];
                entry.IAmAliveTime = iAmAliveTime;
            });
        }

        public Task<bool> UpsertRow(string deploymentId, MembershipEntry entry, string etag, TableVersion tableVersion)
        {
            return Task.Run(() =>
            {
                var coll = Database.GetCollection<Cluster>();
                var cluster = coll.FindById(deploymentId);
                
                
                if(cluster == null)
                {
                    cluster = new Cluster()
                    {
                        DeploymentId = deploymentId,
                        Version = tableVersion.Version,
                        VersionEtag = tableVersion.VersionEtag
                    };                    
                }
                else
                {
                    cluster.Version= tableVersion.Version;
                    cluster.VersionEtag = tableVersion.VersionEtag;
                }

                var m = Member.Create(entry);
                var key = BuildKey(entry.SiloAddress);
                if (!cluster.Members.ContainsKey(key))
                    cluster.Members.Add(key, m);
                else
                    cluster.Members[key] = m;
                
                coll.Upsert(deploymentId, cluster);
                return true;
            });
        }

        private static string BuildKey(SiloAddress address)
        {
            return address.ToParsableString().Replace('.', '_');
        }

        public Task Init(string deploymentId)
        {
            return Task.CompletedTask;
        }
    }
}
