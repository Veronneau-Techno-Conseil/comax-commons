using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using GrainStorageService.Models.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrainStorageService.Models
{
    public class StorageDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        internal readonly DbConf configs;

        public StorageDbContext(IOptionsMonitor<DbConf> options) : base()
        {
            configs = options.CurrentValue;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            DbConfiguration.Setup(builder);
        }

        //the below line was added because accessing the dbsets from controllers where not working
        //when added, everything worked well.. to be validated
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);


            if (configs.MemoryDb)
            {
                optionsBuilder.UseInMemoryDatabase("AccountsDb");
            }
            else
            {
                var cs = $"server={configs.Server};port={configs.Port};user={configs.Username};password={configs.Password};database={configs.Database}";
                var version = ServerVersion.AutoDetect(cs);
                optionsBuilder.UseMySql(cs, version, mysqloptions=>
                        mysqloptions.UseNewtonsoftJson(MySqlCommonJsonChangeTrackingOptions.RootPropertyOnly)
                    )
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableDetailedErrors();
            }
        }



        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

        }


        private class NullConverter : ValueConverter<string, string>
        {
            public NullConverter()
                : base(
                    v => ConvertString(v),
                    v => ConvertString(v))
            {
            }
            public override bool ConvertsNulls => false;

            static string ConvertString(string s)
            {
                if (s == null || s.Equals("[null]", StringComparison.InvariantCultureIgnoreCase) || s.Equals("(null)", StringComparison.InvariantCultureIgnoreCase))
                    return null;
                return s;
            }
        }
    }
}
