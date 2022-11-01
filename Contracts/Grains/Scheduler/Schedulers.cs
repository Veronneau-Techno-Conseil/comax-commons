using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Scheduler
{
    public class Schedulers
    {
        public string ID { get; set; }
        public string DataSourceID { get; set; }
        public string CronExpression { get; set; }
        public string CronTimeZone { get; set; }
        public DateTime NextExecutionTime { get; set; }
        public string ScheduleType { get; set; }
        public DateTime StartDate { get; set; }
    }
}
