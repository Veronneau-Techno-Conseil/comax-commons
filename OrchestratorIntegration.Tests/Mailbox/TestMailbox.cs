using Comax.Commons.Orchestrator.Client;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using CommunAxiom.Commons.Shared.OIDC;

namespace OrchestratorIntegration.Tests.Mailbox
{
    [TestFixture]
    public class TestMailbox
    {
        [Test]
        public async Task TestGrainConnect()
        {
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);

            await cf.WithClusterClient(async client =>
            {
                var mb = client.GetMailbox("Test mailbox");
                var testCall = mb.HasMail();
                await testCall;
                testCall.Exception.Should().BeNull("Task should complete successfully");
            });
        }
    }
}
