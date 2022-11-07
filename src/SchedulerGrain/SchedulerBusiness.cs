using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunAxiom.Commons.Orleans;
using Cronos;

namespace SchedulerGrain
{
    public class SchedulerBusiness
    {
        private SchedulerRepo _schedulerRepo;
        private readonly IComaxGrainFactory _grainFactory;

        public SchedulerBusiness(IComaxGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        public void Init(IPersistentState<Schedulers> scheduler, IPersistentState<SchedulersList> schedulersList)
        {
            _schedulerRepo = new SchedulerRepo(scheduler, schedulersList);
        }

        public async Task<bool> CheckSchedulerslist()
        {
            return await _schedulerRepo.SchedulerListIsSet();
        }

        public async Task<SchedulersList> CreateSchedulersList()
        {
            var listCreated = await _schedulerRepo.CreateSchedulersList();
            if (listCreated.Schedulers != null)
            {
                return listCreated;
            }
            return await Task.FromResult<SchedulersList>(null);
        }

        public async Task<IEnumerable<Schedulers>> GetSchedulersList()
        {
            var ListCreated = await CheckSchedulerslist();
            if (ListCreated != true)
            {
                await CreateSchedulersList();
            }
            var portfoliosList = await _schedulerRepo.GetSchedulersList();
            if (portfoliosList.Schedulers != null)
            {
                return portfoliosList.Schedulers;
            }
            return await Task.FromResult<IEnumerable<Schedulers>>(null);
        }

        public async Task AddAScheduler(Schedulers scheduler)
        {
            if (scheduler != null)
            {
                var listCreated = await CheckSchedulerslist();
                if (listCreated != true)
                {
                    await CreateSchedulersList();
                }
                scheduler.NextExecutionTime = await CalculateNextExecution(scheduler.CronExpression);
                await _schedulerRepo.AddAScheduler(scheduler);
            }
        }

        public async Task<Schedulers> GetASchedulerDetails(string schedulerID)
        {
            var portfolio = await _schedulerRepo.GetASchedulerDetails(schedulerID);
            if (portfolio != null)
            {
                return portfolio;
            }
            return await Task.FromResult<Schedulers>(null);
        }

        public async Task ExecuteAndUpdateScheduled()
        {
            var listCreated = await CheckSchedulerslist();
            if (listCreated != true)
            {
                await CreateSchedulersList();
            }
            var dueSchedulers = await _schedulerRepo.GetDueSchedulers();

            if (dueSchedulers == null)
                return;

            foreach (var scheduler in dueSchedulers)
            {
                //call ingestion grain here
                var ingestionGrain = _grainFactory.GetGrain<IIngestion>(scheduler.DataSourceID);
                await ingestionGrain.Run();

                //Update next occurrence
                var nextExecution = await CalculateNextExecution(scheduler.CronExpression);
                await _schedulerRepo.UpdateNextOccurrence(scheduler.ID, nextExecution);
            }
        }

        public async Task<DateTime> CalculateNextExecution(string cronExpression)
        {
            //calculate next execution with Cronos: https://github.com/HangfireIO/Cronos#adding-seconds-to-an-expression 
            var cron = CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
            return (DateTime)cron.GetNextOccurrence(DateTime.UtcNow);
        }
    }
}
