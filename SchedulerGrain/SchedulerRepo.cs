using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using System.Linq;
using Cronos;

namespace SchedulerGrain
{
    public class SchedulerRepo
    {
        private readonly IPersistentState<Schedulers> _scheduler;
        private readonly IPersistentState<SchedulersList> _schedulersList;

        public SchedulerRepo(IPersistentState<Schedulers> scheduler, IPersistentState<SchedulersList> schedulersList)
        {
            _scheduler = scheduler;
            _schedulersList = schedulersList;
        }

        public async Task<SchedulersList> GetSchedulersList()
        {
            await _schedulersList.ReadStateAsync();
            return _schedulersList.State;
        }
        public async Task<bool> SchedulerListIsSet()
        {
            var res = await GetSchedulersList();
            return res != null;
        }

        public async Task<SchedulersList> CreateSchedulersList()
        {
            var scheduler = new SchedulersList();
            _schedulersList.State = scheduler;
            await _scheduler.WriteStateAsync();
            var listDetails = await GetSchedulersList();
            return listDetails;
        }

        public async Task AddAScheduler(Schedulers scheduler)
        {
            var schedulersList = await GetSchedulersList();
            if (schedulersList.Schedulers == null)
            {
                schedulersList.Schedulers = new List<Schedulers>();
            }

            //calculate the next occurrence based on the expression and the current timing
            //the cronos library has been userd here and can be reached at the below link:
            //https://github.com/HangfireIO/Cronos#adding-seconds-to-an-expression 
            var parsedScheduler = scheduler;
            CronExpression cron = CronExpression.Parse(scheduler.CronExpression, CronFormat.IncludeSeconds);
            parsedScheduler.NextExecutionTime = (DateTime)cron.GetNextOccurrence(DateTime.UtcNow);

            //concat the parsedScheduler with next occurence to the list
            schedulersList.Schedulers = schedulersList.Schedulers.Concat(new[] { parsedScheduler });
            await _schedulersList.WriteStateAsync();
        }

        public async Task<Schedulers> GetASchedulerDetails(string schedulerID)
        {
            var schedulersList = await GetSchedulersList();
            var scheduler = schedulersList.Schedulers.AsQueryable().Where(x => x.ID == schedulerID).FirstOrDefault();
            return (scheduler);
        }

        public async Task UpdateScheduler(string schedulerID)
        {
            var schedulersList = await GetSchedulersList();

            var nextExecution = DateTime.UtcNow.AddSeconds(10);
            foreach (var scheduler in schedulersList.Schedulers.Where(x => x.ID == schedulerID))
                scheduler.NextExecutionTime = nextExecution;
            await _schedulersList.WriteStateAsync();

            //to view the updated list
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var scheduler in schedulersList.Schedulers)
            {
                Console.WriteLine("\nID: " + scheduler.ID + " Expression: " +
                    scheduler.CronExpression + " Next Execution: " + scheduler.NextExecutionTime);
            }
        }
    }
}
