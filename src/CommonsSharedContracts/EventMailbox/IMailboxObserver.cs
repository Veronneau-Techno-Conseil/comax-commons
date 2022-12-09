using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox
{
    public interface IMailboxObserver : IGrainObserver
    {
        Task NewMail(MailMessage message);
    }
}
