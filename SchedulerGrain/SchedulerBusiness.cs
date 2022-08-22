using CommunAxiom.Commons.Client.Contracts.Grains.Scheduler;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerGrain
{
    public class SchedulerBusiness
    {
        private readonly IConfiguration _configuration;
        private SchedulerRepo _schedulerRepo;

        public SchedulerBusiness(IConfiguration configuration)
        {
            this._configuration = configuration;
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
    }
}
