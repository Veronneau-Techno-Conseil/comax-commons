using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Events
{
    public class Subscription
    {
        public EventType EventType { get; set; }
        public TargetTypes Targets { get; set; }
        public string Originator { get; set; }
        public bool Wildcard { get; set; }
        public bool IsAuthorized { get; set; }
        public DateTimeOffset LastAuthorized { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
