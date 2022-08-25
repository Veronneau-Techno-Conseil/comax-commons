using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.IO;
using CommunAxiom.Commons.Client.Grains.DatasourceGrain;
using GrainTests.Shared;
using NUnit.Framework;
using Orleans.Runtime;

namespace Ingestion.Grain.Tests
{
    [TestFixture]
    public class BusinessTest
    {

        [Test]
        public async Task RunWhenNoErrors()
        {           
            IPersistentState<SourceState> store = new PersistentStorageMock<SourceState>(); ;
            
            var business = new Business(new Repo(store));

            await business.WriteConfig(new SourceState
            {
                DataSourceType = DataSourceType.File,
                Configurations = new Dictionary<string, DataSourceConfiguration>()
                {
                    ["Name"] = new DataSourceConfiguration
                    {
                        Name = "Name"
                    }
                }
            });
            var state = await business.ReadConfig();

            Assert.IsNotNull(state);
            Assert.AreEqual(DataSourceType.File, state.DataSourceType);
            Assert.AreEqual("Name", state.Configurations["Name"].Name);

            await business.DeleteConfig();
            state = await business.ReadConfig();
            Assert.IsNull(state);

        }
    }
}