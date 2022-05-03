using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts
{
    public class SystemEvent
    {
        public SystemEventType Type { get; set; }
        public string Message { get; set; }
        public string Payload { get; set; }
    }

    public enum SystemEventType
    {
        ClusterAuthenticationComplete = 0,
        ClusterAuthenticationReset = 1,
    }
}
