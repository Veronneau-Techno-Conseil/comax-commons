using Comax.Commons.Orchestrator.MembershipProvider;
using Comax.Commons.Orchestrator.MongoDbMembershipStorage;
using Orleans.Configuration;
using Orleans;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using Comax.Commons.Orchestrator.LiteDbMembershipStorage;

namespace AgentReferee
{
    public class RefereeApp : CommunAxiom.Commons.CommonsShared.MembershipApi.Application
    {
        public RefereeApp(string configFile = "./config.json") : base(configFile)
        {

        }

        public override void SetStorage()
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            if (_config["db"] == "mongo")
            {
                _hostBuilder.ConfigureServices(services =>
                {
                    services.AddSingleton<IMongoClientFactory>(sp =>
                    {
                        return new MongoClientFactory(_config);
                    });

                    services.Configure<MongoDBOptions>(mo =>
                    {
                        mo.DatabaseName = "clustermembers";
                        mo.ClientName = "member_mongo";
                        mo.CollectionConfigurator = cs =>
                        {
                            cs.WriteConcern = WriteConcern.Acknowledged;
                            cs.ReadConcern = ReadConcern.Local;
                        };

                    });

                    services.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = _config["ClusterId"];
                        options.ServiceId = _config["ServiceId"];
                    });

                    services.SetMongoDbMembership();
                });
            }
            else
            {
                _hostBuilder.ConfigureServices(services =>
                {
                    services.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = _config["ClusterId"];
                        options.ServiceId = _config["ServiceId"];
                    });
                    services.SetLiteDbMembership(_config.GetSection("LiteDb"));
                });
            }

        }

        //TODO Set security for commons app access
        public override void ActorPolicyAssertion(AuthorizationPolicyBuilder builder)
        {
            builder.RequireAssertion(ctxt =>
            {
                return ctxt.User != null;
            });
        }

        public override void AuthenticatedPolicyAssertion(AuthorizationPolicyBuilder builder)
        {
            builder.RequireAssertion(ctxt =>
            {
                return ctxt.User != null;
            });
        }

        public override void ReaderPolicyAssertion(AuthorizationPolicyBuilder builder)
        {
            builder.RequireAssertion(ctxt =>
            {
                return ctxt.User != null;
            });
        }
    }
}
