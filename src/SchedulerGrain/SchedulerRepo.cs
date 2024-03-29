﻿using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using System.Linq;
using Orleans;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;

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
            schedulersList.Schedulers = schedulersList.Schedulers.Concat(new[] { scheduler });
            await _schedulersList.WriteStateAsync();
        }

        public async Task<Schedulers> GetASchedulerDetails(string schedulerID)
        {
            var schedulersList = await GetSchedulersList();
            return schedulersList.Schedulers.AsQueryable().Where(x => x.ID == schedulerID).FirstOrDefault();
        }

        public async Task<IEnumerable<Schedulers>> GetDueSchedulers()
        {
            var schedulersList = await GetSchedulersList();
            if (schedulersList.Schedulers == null)
                return null;
            var filteredSchedulers = schedulersList.Schedulers.AsQueryable().Where(x => x.NextExecutionTime.ToUniversalTime() <= DateTime.UtcNow);
            return filteredSchedulers;
        }

        public async Task UpdateNextOccurrence(string schedulerID, DateTime nextOccurrence)
        {
            var schedulersList = await GetSchedulersList();
            foreach (var oldScheduler in schedulersList.Schedulers.Where(x => x.ID == schedulerID))
                oldScheduler.NextExecutionTime = (DateTime)nextOccurrence;
            await _schedulersList.WriteStateAsync();
        }
    }
}
