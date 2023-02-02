using Comax.Commons.Orchestrator.MembershipProvider;
using Comax.Commons.Orchestrator.MembershipProvider.Models;
using static Comax.Commons.Orchestrator.LiteDbMembershipStorage.LiteDbMembershipStorageProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.LiteDbMembershipStorage
{
    public static class MongoDbMembershipStorageExtensions
    {
        public static void SetLiteDbMembership(this IServiceCollection serviceCollection, IConfiguration config)
        {
            LiteDbConfig cfg = new LiteDbConfig();
            config.Bind(cfg);
            serviceCollection.AddSingleton(cfg);
            serviceCollection.AddSingleton<IMembershipStorage, LiteDbMembershipStorageProvider>();
        }
    }
}
