using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Events
{
    [Flags]
    public enum TargetTypes
    {
        None = 0,
        System = 1,
        User=1<<1,
        CommonsClient=1<<2,
    }
}
