using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests
{
  [TestFixture]
  public class TestMongoConnect
  {
        [Test]
        public async Task TestConnect()
        {
            //var f = Cluster.ServiceProvider.GetService<IMongoClientFactory>();
            //var client = f.Create("");
            //var db = client.GetDatabase("clustermembers");
            //var cols = await db.ListCollectionNamesAsync();
            //cols.Should().NotBeNull();
            
        }
  }
}
