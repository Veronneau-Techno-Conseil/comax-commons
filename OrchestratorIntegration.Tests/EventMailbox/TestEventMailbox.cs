using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Client;
using Comax.Commons.Orchestrator.MailGrain;
using NUnit.Framework;
using Shared.Tests.RulesEngine.Mock;

namespace OrchestratorIntegration.Tests.EventMailbox
{
    [TestFixture]
    public class TestEventMailbox
    {
        
        [Test]
        public async Task TestSendMail()
        {
            MessageRulesEngineMock messageRulesEngineMock = new MessageRulesEngineMock();
            var mail = new MailMessage()
            {
                From = "com://local",
                MsgId = Guid.NewGuid(),
                Type = "NEW_DATA_VERSION",
                Subject = "test EventMailbox message"
            };
            //var res = messageRulesEngineMock.Validate(mail);
        }
    }
}
