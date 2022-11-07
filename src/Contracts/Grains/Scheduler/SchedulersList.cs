using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Scheduler
{
    public class SchedulersList
    {
        public IEnumerable<Schedulers> Schedulers { get; set; }
    }
}
