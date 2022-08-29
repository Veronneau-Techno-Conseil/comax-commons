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
using CommunAxiom.Commons.Client.Contracts.Ingestion;

namespace SchedulerGrain
{
    [GrainDirectory(GrainDirectoryName = "MyGrainDirectory")]
    public class Scheduler : Grain, IScheduler
    {
        private readonly SchedulerBusiness _schedulerBusiness;
        private IDisposable _timer, _checkEachSecond;
        private IIngestion ingestion;

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
            _checkEachSecond = RegisterTimer(x => CheckAndLaunchSchedulers(), true, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10)); // to serve approach 2
            return base.OnActivateAsync();
        }

        public async Task CheckAndLaunchSchedulers()
        {
            var schedulers = await GetDueSchedulers();
            if (schedulers != null)
            {
                await ExecuteScheduledJob(schedulers);
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
            await _schedulerBusiness.AddAScheduler(scheduler);
        }

        public async Task<IEnumerable<Schedulers>> GetAllSchedulers()
        {
            return await _schedulerBusiness.GetSchedulersList();
        }

        public async Task<IEnumerable<Schedulers>> GetDueSchedulers()
        {
            return await _schedulerBusiness.GetDueSchedulersList();
        }

        public async Task<Schedulers> GetAScheduler(string schedulerId)
        {
            return await _schedulerBusiness.GetASchedulerDetails(schedulerId);
        }

        public async Task ExecuteScheduledJob(IEnumerable<Schedulers> schedulers)
        {
            foreach (var scheduler in schedulers)
            {
                //call the ingestion here
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nCall " + scheduler.ID + " at: " + DateTime.Now.ToString() + "\n");

                //await ingestion.Run();
                await _schedulerBusiness.UpdateScheduler(scheduler.ID, scheduler.CronExpression);
            }
        }
    }
}
