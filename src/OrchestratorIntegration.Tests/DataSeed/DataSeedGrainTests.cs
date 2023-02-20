using Comax.Commons.Orchestrator.Client;
using Comax.Commons.Orchestrator.Contracts.Seeding;
using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;
using CommunAxiom.Commons.CommonsShared.Contracts.DataSeed;
using CommunAxiom.Commons.Shared;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests.DataSeed
{
    [TestFixture]
    public class DataSeedGrainTests
    {
        [Test]
        public async Task CreateIndex()
        {
            Guid newid = Guid.NewGuid();
            Guid streamId = Guid.NewGuid();
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);

            var response = await cf.WithClusterClient(async cl =>
            {
                var ds = cl.GetDataSeed(newid);
                var res = await ds.SetUploadStream(streamId);
                return res;
            });

            response.Should().NotBeNull();
            response.Error.Should().BeEquivalentTo(OperationResult.ERR_NOT_FOUND, "the index was not created");

            var ix = new DataIndex
            {
                Id = newid,
                Index = new List<DataIndexItem>
                {
                    new DataIndexItem
                    {
                        Id = $"{newid}-data-1",
                        IndexData = new Dictionary<string, string>{ }
                    },
                    new DataIndexItem
                    {
                        Id = $"{newid}-data-2",
                        IndexData = new Dictionary<string, string>{ }
                    }
                }
            };

            var createdIx = await cf.WithClusterClient(async cl =>
            {
                var ds = cl.GetDataSeed(newid);
                await ds.SendIndex(ix);
                return await ds.GetIndex();
            });        

            createdIx.Should().NotBeNull();
            createdIx.Id.Should().Be(ix.Id);

            List<DataChunkObject> objects = new List<DataChunkObject>()
            {
                new DataChunkObject
                {
                    Id =$"{newid}-data-1",
                    Data = JObject.FromObject(new Dummy
                    {
                        Name=$"{newid}-data-1 Name",
                        Description=$"{newid}-data-1 Description"
                    })
                },
                new DataChunkObject
                {
                    Id =$"{newid}-data-2",
                    Data = JObject.FromObject(new Dummy
                    {
                        Name=$"{newid}-data-2 Name",
                        Description=$"{newid}-data-2 Description"
                    })
                }
            };

            await cf.WithClusterClient(async cl =>
            {
                var ds = cl.GetDataSeed(newid);
                await ds.SetUploadStream(streamId);
                var strm = cl.GetDataChunkStream(streamId);

                foreach(var o in objects)
                {
                    await strm.OnNextAsync(o);
                }

                await strm.OnCompletedAsync();
            });

            //var res = await cf.WithClusterClient(async cl =>
            //{
            //    var ds = cl.GetDataSeed(newid);
            //    var res = await ds.StreamDataFromStorage(streamId);
            //    return res;
            //});
            //res.Should().NotBeNull();
        }
    }
}
