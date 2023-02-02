using Comax.Commons.Orchestrator.Client;
using CommunAxiom.Commons.Orleans.Security;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests
{
    [TestFixture]
    public class TestMailbox
    {
        [Test]
        public async Task TestGrainConnect()
        {
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
            await cf.WithClusterClient(async c =>
            {
                var mb = await c.GetEventMailbox(Guid.Empty);
                var testCall = await mb.HasMail();
            });
        }

        [Test]
        public async Task TestGrainConnectNoAuth()
        {
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
            Cluster.NoAuth = true;
            try
            {
                await cf.WithClusterClient(async c =>
                {
                    var mb = await c.GetEventMailbox(Guid.Empty);
                    var testCall = await mb.HasMail();
                });

                Assert.Fail("Unauthorized call should have thrown.");
            }
            catch(Exception e)
            {
                Assert.IsTrue(e is AccessControlException);
            }
            finally
            {
                Cluster.NoAuth = false;
            }
        }
    }
}
