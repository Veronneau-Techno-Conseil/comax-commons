using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cronos;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using System.Linq;

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
            var parsedScheduler = scheduler;
            CronExpression cron = CronExpression.Parse(scheduler.CronExpression);
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

        //use to parse a cron expression
        public Task<string> ParseCron(string cronExpression)
        {
            CronExpression expression = CronExpression.Parse(cronExpression);
            DateTime? nextUtc = expression.GetNextOccurrence(DateTime.UtcNow);
            Console.Write(nextUtc.ToString());
            return Task.FromResult(nextUtc.ToString());
        }
    }
}
