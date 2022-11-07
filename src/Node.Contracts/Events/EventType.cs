using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Events
{
    [Flags]
    public enum EventType
    {
        None=0,
        System=1,
        PersonalMessage = 1<<1,
    }
}
