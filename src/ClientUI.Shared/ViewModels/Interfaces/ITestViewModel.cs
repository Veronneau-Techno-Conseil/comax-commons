
using CommunAxiom.Commons.ClientUI.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class has been added just for testing purposes and shall be deleted later on

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces
{
    public interface ITestViewModel
    {
        string ID { get; set; }
        string DataSourceID { get; set; }
        string CronExpression { get; set; }
        string CronTimeZone { get; set; }
        string NextExecutionTime { get; set; }
        string ScheduleType { get; set; }
        DateTime StartDate { get; set; }
        Schedulers scheduler { get; set; }
        List<Schedulers> Schedulers { get; set; }
        Task CreateScheduler(Schedulers scheduler);
        Task<List<Schedulers>?> GetSchedulers();
    }
}
