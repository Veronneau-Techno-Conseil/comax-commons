using Comax.Commons.Orchestrator.Client;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Shared.RulesEngine;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests.Actor
{
    [TestFixture]
    public class IAmAliveTests : IMailboxObserver
    {
        List<MailMessage> _mailMessages = new List<MailMessage>();
        public Task NewMail(MailMessage message)
        {
            _mailMessages.Add(message);
            return Task.CompletedTask;
        }

        [Test]
        public async Task TestAlive()
        {
            try
            {
                Cluster.AsCommonsAgent = true;

                var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
                await cf.WithClusterClient(async c =>
                {
                    var mb = await c.GetEventMailbox();
                    var disposable = await mb.Subscribe(this);

                    var act = await c.GetActor();
                    await act.IAmAlive();

                    await GrainTests.Shared.Helper.SpinDelay(() => _mailMessages.Any(x => x.Type == MessageTypes.OrchestratorInstructions.MSG_TYPE_ACK_ALIVE), 2000, 15);
                    _mailMessages.Should().HaveCountGreaterThan(1);
                    _mailMessages.Should().Contain(m => m.Type == MessageTypes.OrchestratorInstructions.MSG_TYPE_ACK_ALIVE);
                });
            }
            finally 
            { 
                Cluster.AsCommonsAgent = false;
            }
        }
    }
}
