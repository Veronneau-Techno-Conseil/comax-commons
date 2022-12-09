using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public class UserAuthState
    {
        public string PrincipalId { get; set; }
        public bool IsAuthorised { get; set; }
        public Guid Subscription { get; set; }
    }
}
