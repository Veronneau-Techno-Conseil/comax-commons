using Comax.Commons.Orchestrator.Client;
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
    }
}
