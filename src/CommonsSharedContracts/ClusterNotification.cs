using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts
{
    public class ClusterNotification
    {
        public const string NEW_MAIL_VERB = "NEW_MAIL";
        public string Verb { get; set; }
        public string Value { get; set; }
    }
}
