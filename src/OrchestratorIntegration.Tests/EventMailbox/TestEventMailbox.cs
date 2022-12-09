using Comax.Commons.Orchestrator.Client;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using FluentAssertions;
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
                To = "com://local",
                MsgId = Guid.NewGuid(),
                Type = "NEW_DATA_VERSION",
                Subject = "test EventMailbox message"
            };
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
            await cf.WithClusterClient(async c =>
            {
                var emb = await c.GetEventMailbox(Guid.NewGuid());
                var obs = new MBObserver();
                var streamId = emb.Subscribe(obs);
                //var testCall = await emb.SendMail();
                await emb.SendMail(mail);
                //unsubscribe from stream

                
                int cnt = 0;
                while (obs.MailMessages.Count < 1 && cnt < 10)
                {
                    await Task.Run(() => Thread.Sleep(10000));
                    cnt++;
                }

                foreach (var item in obs.MailMessages)
                {
                    await emb.DeleteMail(item.MsgId);
                }

                obs.MailMessages.Should().NotBeNullOrEmpty();
            });            
        }


    }

    public class MBObserver: IMailboxObserver
    {
        public List<MailMessage> MailMessages { get; set; } = new List<MailMessage>();
        public Task NewMail(MailMessage message)
        {
            this.MailMessages.Add(message);
            return Task.CompletedTask;
        }
    }
}
