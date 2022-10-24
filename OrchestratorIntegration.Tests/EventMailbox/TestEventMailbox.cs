using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Client;
using Comax.Commons.Orchestrator.MailGrain;
using NUnit.Framework;
using Orleans.Streams;

namespace OrchestratorIntegration.Tests.EventMailbox
{
    [TestFixture]
    public class TestEventMailbox
    {

        [Test]
        public async Task TestStreamSubscription()
        {
            var mail = new MailMessage()
            {
                From = "com://local",
                MsgId = Guid.NewGuid(),
                Type = "NEW_DATA_VERSION",
                Subject = "test EventMailbox message"
            };
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
            await cf.WithClusterClient(async c =>
            {
                var emb = c.GetEventMailbox(Guid.NewGuid());
                var streamId = emb.GetStreamId();
                //var testCall = await emb.SendMail();
                var subscriptionHandle = c.SubscribeEventMailboxStream(streamId.Result, OnMailMessage, OnEventMailboxStreamError, OnComplete);
                await emb.SendMail(mail);
                //unsubscribe from stream
                
            });
            
        }

        public Task OnMailMessage(MailMessage mail,StreamSequenceToken token)
        {
            return Task.CompletedTask;
        }
        public Task OnEventMailboxStreamError(Exception e)
        {
            Console.Error.WriteLine(e.ToString());
            Console.Error.WriteLine(e.StackTrace);
            return Task.CompletedTask;
        }
        public Task OnComplete()
        {
            return Task.CompletedTask;
        }

    }
}
