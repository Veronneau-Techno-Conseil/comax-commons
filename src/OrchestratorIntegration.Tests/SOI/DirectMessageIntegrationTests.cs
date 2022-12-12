using Comax.Commons.Orchestrator.Client;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Shared.RulesEngine;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
                    To = contacts[0].Uri,
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
                var reg = client.GetUriRegistry(c.Uri);
                var userID = await reg.GetOrCreate();
                var mb = await client.GetEventMailbox(userID);

                int cnt = 0;
                int msgCnt = 0;
                var testCall = await mb.HasMail();

                while (!testCall && cnt < 10)
                {
                    Thread.Sleep(200);
                    cnt++;
                    testCall = await mb.HasMail();
                }
                testCall.Should().BeTrue();
                cnt = 0;
                
                List<MailMessage> msgs = new List<MailMessage>();
                var obs = new DelegateMbObserver((mm) =>
                {
                    msgCnt++;
                    mm.Should().NotBeNull();
                    msgs.Add(mm);
                });

                await mb.Subscribe(obs);
                
                while(msgCnt < 1 && cnt < 10){
                    await Task.Run(() => Thread.Sleep(10000));
                    cnt++;
                }

                foreach(var item in msgs)
                {
                    await mb.DeleteMail(item.MsgId);
                }
                
                msgCnt.Should().BeGreaterThan(0);
            });
        }
    }

    public class DelegateMbObserver : IMailboxObserver
    {
        private readonly Action<MailMessage> _action;
        public DelegateMbObserver(Action<MailMessage> action) 
        { 
            _action = action;
        }

        public Task NewMail(MailMessage message)
        {
            _action(message);
            return Task.CompletedTask;
        }
    }
}
