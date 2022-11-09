using Comax.Commons.Orchestrator.Client;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans;
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
                int msgCnt = 0;
                var handle = await client.SubscribeEventMailboxStream(strmid,
                    (mm, sst) =>
                    {
                        msgCnt++;
                        mm.Should().NotBeNull();
                        return Task.CompletedTask;
                    }, ex =>
                    {
                        Assert.Fail($"Should not error - {ex}");
                        return Task.CompletedTask;
                    }, () =>
                    {
                        return Task.CompletedTask;
                    });

                var (hdl, enumerable) = await client.EnumEventMailbox(strmid);

                _ = Task.Run(async () =>
                {
                    await foreach (var item in enumerable)
                    {
                        msgCnt++;
                        await mb.DeleteMail(item.MsgId);
                    }
                });

                var strm = client.ClusterClient.GetStreamProvider(Constants.DefaultStream).GetStream<MailMessage>(strmid, Constants.DefaultNamespace);
                await strm.OnNextAsync(new MailMessage
                {
                    From = "test"
                });

                await mb.StartStream();
                
                int cnt = 0;
                while(msgCnt < 2 && cnt < 10){
                    await Task.Run(() => Thread.Sleep(10000));
                    cnt++;
                }
                await strm.OnCompletedAsync();
                msgCnt.Should().BeGreaterThan(0);
            });
        }
    }
}
