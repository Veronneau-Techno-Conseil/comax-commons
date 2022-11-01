using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class Schedulers
    {
        public string? ID { get; set; }
        public string? DataSourceID { get; set; }
        public string? CronExpression { get; set; }
        public string? CronTimeZone { get; set; }
        public DateTime NextExecutionTime { get; set; }
        public string? ScheduleType { get; set; }
        public DateTime StartDate { get; set; }
    }
}
