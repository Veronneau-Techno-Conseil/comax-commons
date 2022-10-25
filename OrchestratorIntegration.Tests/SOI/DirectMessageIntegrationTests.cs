using Comax.Commons.Orchestrator.Client;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests.SOI
{
    [TestFixture]
    public class DirectMessageIntegrationTests
    {
        [Test]
        public async Task TestMessage()
        {
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);

            await cf.WithClusterClient(async client =>
            {
                var central = client.GetCentral();
                var contacts = (await central.GetContacts()).ToArray();

                //Broadcast
                var user = await client.GetUriRegistry("").GetCurrentUser();
                var msg = new Message
                {
                    Id = Guid.NewGuid(),
                    From = MessageHelper.UserUri(user.Id),
                    To = MessageHelper.UserUri(contacts[0].Id),
                    CreatedDate = DateTime.UtcNow,
                    FromOwner = MessageHelper.UserUri(user.Id),
                    Payload = "Test message body",
                    Scope = MessageHelper.MSG_SCOPE_PRIVATE,
                    Subject = "Test message subject",
                    Type = MessageHelper.MSG_TYPE_DIRECT
                };
                var soi = await client.GetSubjectOfInterest();
                await soi.Broadcast(msg);

                //Fetch message
                var c = contacts[0];
                var reg = client.GetUriRegistry(c.Id);
                var userID = await reg.GetOrCreate();
                var mb = await client.GetEventMailbox(userID);
                var testCall = await mb.HasMail();
                testCall.Should().BeTrue();
                var strmid = await mb.GetStreamId();
                
            });
        }
    }
}
