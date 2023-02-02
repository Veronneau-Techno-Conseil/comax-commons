using Comax.Commons.Orchestrator.MembershipProvider;
using Comax.Commons.Orchestrator.MembershipProvider.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.MongoDbMembershipStorage
{
    public static class MongoDbMembershipStorageExtensions
    {
        public static void SetMongoDbMembership(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMongoClientFactory, MongoClientFactory>();
            serviceCollection.AddSingleton<IMembershipStorage, MongoMembershipStorage>();
        }
    }
}
