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

namespace SchedulerGrain
{
    [GrainDirectory(GrainDirectoryName = "MyGrainDirectory")]
    public class Scheduler : Grain, IScheduler
    {
        private readonly SchedulerBusiness _schedulerBusiness;
        private IDisposable timer;

        public Scheduler(IConfiguration configuration, SchedulerBusiness schedulerBusiness,
            [PersistentState("scheduler")] IPersistentState<Schedulers> scheduler,
            [PersistentState("schedulersList")] IPersistentState<SchedulersList> schedulersList)
        {
            _schedulerBusiness = schedulerBusiness;
            _schedulerBusiness.Init(scheduler, schedulersList);
        }

        public override Task OnActivateAsync()
        {
            timer = RegisterTimer(x => DelayIt(), true, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            return base.OnActivateAsync();
        }

        public Task DelayIt()
        {
            DelayDeactivation(TimeSpan.FromSeconds(10));
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("****\nHitted Delay\n*******\n");
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

        public async Task<Schedulers> GetAScheduler(string schedulerId)
        {
            return await _schedulerBusiness.GetASchedulerDetails(schedulerId);
        }
    }
}
