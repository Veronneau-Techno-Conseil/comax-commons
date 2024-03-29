﻿using GrainStorageService.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrainStorageService.Models.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static void MigrateDb(this IServiceProvider services)
        {
            var sp = services;
            using var scope = sp.CreateScope();

            var serviceProvider = scope.ServiceProvider;


            var dbConf = serviceProvider.GetService<IOptionsMonitor<DbConf>>().CurrentValue;

            if (dbConf.MemoryDb || !dbConf.ShouldMigrate)
            {
                return;
            }

            var context = scope.ServiceProvider.GetRequiredService<StorageDbContext>();

            var dbcontext = serviceProvider.GetService<StorageDbContext>();

            if (dbConf.ShouldDrop)
            {
                dbcontext.Database.EnsureDeleted();
            }

            dbcontext.Database.Migrate();
            
        }
    }
}
