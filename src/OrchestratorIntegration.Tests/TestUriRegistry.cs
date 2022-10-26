using Comax.Commons.Orchestrator.Client;
using NUnit.Framework;

namespace OrchestratorIntegration.Tests
{
    [TestFixture]
    public class TestUriRegistry
    {
        [Test]
        public async Task TestGrainConnect()
        {
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
            await cf.WithClusterClient(async c =>
            {
                var ur = c.GetUriRegistry("Test uri");
                var testCall = await ur.GetOrCreate();
            });
        }
    }
}
