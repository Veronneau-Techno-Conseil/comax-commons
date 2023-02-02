using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox
{
    public class MailboxState
    {
        public List<MailMessage> MailMessages { get; set; } = new List<MailMessage>();
    }
}
