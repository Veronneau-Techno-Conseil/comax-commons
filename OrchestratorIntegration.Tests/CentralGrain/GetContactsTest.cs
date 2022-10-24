using Comax.Commons.Orchestrator.Client;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests.CentralGrain
{
    [TestFixture]
    public class GetContactsTest
    {
        [Test]
        public async Task GetContacts()
        {
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);

            await cf.WithClusterClient(async client =>
            {
                try
                {
                    var central = client.GetCentral();
                    var testCall = central.GetContacts();
                    await testCall;
                    testCall.Exception.Should().BeNull("Task should complete successfully");
                    testCall.Result.Should().NotBeNull();
                    var res = testCall.Result;
                    res.ToList().Count.Should().BeGreaterThan(0);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.ToString());
                }
            });
        }
    }
}
