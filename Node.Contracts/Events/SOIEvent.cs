using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Events
{
    /// <summary>
    /// Subject of interest event
    /// </summary>
    public class SOIEvent
    {
        public EventType EventType { get; set; }
        public TargetTypes Targets { get; set; }
        public string[] Recipients { get; set; }
        public string Payload { get; set; }
    }
}
