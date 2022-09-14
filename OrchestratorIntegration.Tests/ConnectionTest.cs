using Comax.Commons.Orchestrator.Client;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests
{
    [TestFixture]
    public class ConnectionTest
    {
        [Test]
        public async Task TestConnect()
        {
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
            var result = await cf.TestConnection();
            result.Should().BeTrue();
        }
    }
}
