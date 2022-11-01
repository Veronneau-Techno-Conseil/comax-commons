using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Scheduler
{
    public interface IScheduler : IGrainWithIntegerKey
    {
        Task AddAScheduler(Schedulers scheduler);
        Task<IEnumerable<Schedulers>> GetAllSchedulers();
        Task<Schedulers> GetAScheduler(string schedulerId);
    }
}
