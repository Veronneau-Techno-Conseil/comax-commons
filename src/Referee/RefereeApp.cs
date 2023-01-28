using Comax.Commons.Orchestrator.MembershipProvider;
using Comax.Commons.Orchestrator.MongoDbMembershipStorage;
using Orleans.Configuration;
using Orleans;
using MongoDB.Driver;

namespace Referee
{
    public class RefereeApp : CommunAxiom.Commons.CommonsShared.MembershipApi.Application
    {
        public RefereeApp(): base("./config.json")
        {

        }
        public override void SetStorage()
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            _hostBuilder.ConfigureServices(svc =>
                svc.AddSingleton<IMongoClientFactory>(sp =>
                {
                    var cfg = sp.GetService<IConfiguration>();
                    return new MongoClientFactory(cfg);
                })
                .Configure<MongoDBOptions>(mo =>
                {
                    mo.DatabaseName = "clustermembers";
                    mo.ClientName = "member_mongo";
                    mo.CollectionConfigurator = cs =>
                    {
                        cs.WriteConcern = WriteConcern.Acknowledged;
                        cs.ReadConcern = ReadConcern.Local;
                    };

                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "0.0.1-a1";
                    options.ServiceId = "OrchestratorCluster";
                })
                .SetMongoDbMembership()
            );
        }
    }
}
