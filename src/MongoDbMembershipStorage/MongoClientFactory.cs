﻿using Comax.Commons.Orchestrator.MembershipProvider.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
//using Orleans.Providers.MongoDB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MongoDbMembershipStorage
{
    public class MongoClientFactory : IMongoClientFactory
    {
        IConfiguration _configuration;
        public MongoClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IMongoClient Create(string name)
        {
            var mmc = new MongoMembershipConfig();
            _configuration.GetSection("member_mongo").Bind(mmc);

            return new MongoClient(new MongoClientSettings
            {
                Credential = MongoCredential.CreateCredential(mmc.authDb, mmc.username, mmc.password),
                AllowInsecureTls = mmc.allowInsecureTls,
                DirectConnection = mmc.directConnection,
                Server = new MongoServerAddress(mmc.host, mmc.port),
                UseTls = mmc.useTls
            });            
        }
    }
}
