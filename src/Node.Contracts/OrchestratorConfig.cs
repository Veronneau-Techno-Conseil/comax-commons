using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts
{
    public class OrchestratorConfig
    {
        public int AckFrequency { get; set; }
        public int PortfolioSyncInterval { get; set; }
        public int PortfolioSyncReminderInterval { get; set; }
    }
}
