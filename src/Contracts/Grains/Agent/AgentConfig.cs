using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public class AgentConfig
    {
        public int LiveTickerPeriod { get; set; }
        public int ConnectionCheckPeriod { get; set; }
        public int SaveStatePeriod { get; set; }
        public int SubscriptionTimeout { get; set; }
    }
}
