using System;
using Orleans;
using System.Threading.Tasks;
using Orleans.GrainDirectory;
using Orleans.Runtime;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Orleans.Concurrency;
using Orleans.Providers;
using Orleans.Runtime.Services;
using Cronos;


namespace SchedulerGrain
{
    [GrainDirectory(GrainDirectoryName = "MyGrainDirectory")]
    public class Scheduler : Grain, IScheduler
    {
        private readonly SchedulerBusiness _schedulerBusiness;
        private IDisposable _timer, _scheduler;

        public Scheduler(IConfiguration configuration, SchedulerBusiness schedulerBusiness,
            [PersistentState("scheduler")] IPersistentState<Schedulers> scheduler,
            [PersistentState("schedulersList")] IPersistentState<SchedulersList> schedulersList)
        {
            _schedulerBusiness = schedulerBusiness;
            _schedulerBusiness.Init(scheduler, schedulersList);
        }

        public override Task OnActivateAsync()
        {
            _timer = RegisterTimer(x => DelayIt(), true, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            LaunchSavedSchedulers();
            return base.OnActivateAsync();
        }

        public async void LaunchSavedSchedulers()
        {
            var schedulers = await GetAllSchedulers();
            if (schedulers != null)
            {
                foreach (var scheduler in schedulers)
                {
                    RegisterScheduler(scheduler.ID, scheduler.CronExpression);
                }
            }
        }

        public Task DelayIt()
        {
            DelayDeactivation(TimeSpan.FromSeconds(10));
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n****\nHitted Delay\n*******\n");
            return Task.FromResult(0);
        }

        public async Task AddAScheduler(Schedulers scheduler)
        {
            RegisterScheduler(scheduler.ID, scheduler.CronExpression);
            await _schedulerBusiness.AddAScheduler(scheduler);
        }

        public async Task<IEnumerable<Schedulers>> GetAllSchedulers()
        {
            return await _schedulerBusiness.GetSchedulersList();
        }

        public async Task<Schedulers> GetAScheduler(string schedulerId)
        {
            return await _schedulerBusiness.GetASchedulerDetails(schedulerId);
        }

        public void RegisterScheduler(string schedulerID, string cronExpression)
        {
            //change the parameter 3 in the below function and replace it with the difference between next execution and now in seconds
            //change the last parameter in the below function and replace it with seconds calculated from cron expression
            var startAfter = ((CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds).GetNextOccurrence(DateTime.UtcNow)) - DateTime.UtcNow).Value.TotalSeconds;
            _scheduler = RegisterTimer(x => ManageScheduler(schedulerID), true, TimeSpan.FromSeconds(startAfter), TimeSpan.FromSeconds(10));
        }

        public async Task ManageScheduler(string schedulerID)
        {
            //replace the below with the call sent to the ingestion grain
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nCall the ingestion function from here. The scheduler with ID: "
                + schedulerID + " is now executed and time is: " + DateTime.Now.ToString() + "\n");

            //to update the schedulers list
            await _schedulerBusiness.UpdateScheduler(schedulerID);

            //we should also send a call from broadcast manager to update the schedulers list in the dashboard
        }
    }
}
