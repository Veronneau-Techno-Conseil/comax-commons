using GrainStorageService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrainStorageService.Models.Configurations
{
    /// <summary>
    /// Initial configuration for any extensions added to the default AspNet and openiddict authentication system
    /// </summary>
    public class InitialConfig : IModelConfig
    {
        public void SetupFields(ModelBuilder builder)
        {
            builder.Entity<StandardStorage>()
                .HasKey("EntityType", "Id");

            builder.Entity<StandardStorage>().HasIndex(new string[] { "EntityType", "Id", "LastModifiedDate" }, "LookupIndex")
                .IsDescending(false, false, true);

            builder.Entity<StandardStorage>()
                .Property(x => x.Id)
                .IsRequired();

            builder.Entity<StandardStorage>()
                .Property(x => x.Storage)
                .IsRequired();

            builder.Entity<StandardStorage>()
                .Property(x => x.LastModifiedDate)
                .IsRequired();

            builder.Entity<StandardStorage>()
                .Property(x => x.CreatedDate)
                .IsRequired();

            builder.Entity<StandardStorage>()
                .Property(x => x.EntityType)
                .IsRequired();
        }

        public void SetupRelationships(ModelBuilder builder)
        {
            
        }

        public void SetupTables(ModelBuilder builder)
        {
            builder.Entity<StandardStorage>()
                .ToTable("StandardStorage");
        }
    }
}
