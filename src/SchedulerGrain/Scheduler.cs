using System;
using Orleans;
using System.Threading.Tasks;
using Orleans.GrainDirectory;
using Orleans.Runtime;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using System.Collections.Generic;
using CommunAxiom.Commons.Orleans;

namespace SchedulerGrain
{
    [GrainDirectory(GrainDirectoryName = "MyGrainDirectory")]
    public class Scheduler : Grain, IScheduler
    {
        private SchedulerBusiness _schedulerBusiness;
        private IDisposable _delayDeActivation, _executeScheduled;
        private IPersistentState<Schedulers> _scheduler;
        private IPersistentState<SchedulersList> _schedulersList;

        public Scheduler([PersistentState("scheduler")] IPersistentState<Schedulers> scheduler,
            [PersistentState("schedulersList")] IPersistentState<SchedulersList> schedulersList)
        {
            _scheduler = scheduler;
            _schedulersList = schedulersList;
        }

        public override Task OnActivateAsync()
        {
            _schedulerBusiness = new SchedulerBusiness(new GrainFactory(this.GrainFactory));
            _schedulerBusiness.Init(_scheduler, _schedulersList);
            _delayDeActivation = RegisterTimer(x => DelayGrainDeactivation(), true,
                TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5)); //Consider changing the timer frequency if needed
            _executeScheduled = RegisterTimer(x => ExecuteAndUpdateScheduled(), true,
                TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)); //Repeat each second to make sure cover every cron job
            return base.OnActivateAsync();
        }

        public Task DelayGrainDeactivation()
        {
            DelayDeactivation(TimeSpan.FromMinutes(5)); //The function is called each 5 minutes to delay deactivation for 5 minutes ==> always active
            return Task.FromResult(0);
        }

        public async Task ExecuteAndUpdateScheduled()
        {
            await _schedulerBusiness.ExecuteAndUpdateScheduled();
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
