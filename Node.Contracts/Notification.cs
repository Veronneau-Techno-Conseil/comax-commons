using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Conmtracts
{
    public class Notification
    {
        public string Recipient { get; set; }
        public string RecipientType { get; set; }
        public string NotificationType { get; set; }
        public string NotificationIdentifier { get; set; }
    }
}
