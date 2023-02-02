using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.CommonsShared.Contracts.DataSeed;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;

namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeedRepo
    {
        private readonly IPersistentState<DataSeedObject> _dataSeedRepo;
        public DataSeedRepo(IPersistentState<DataSeedObject> dataSeedRepo)
        {
            this._dataSeedRepo = dataSeedRepo;
        }
        public async Task<DataSeedObject> Fetch()
        {
            await _dataSeedRepo.ReadStateAsync();
            return _dataSeedRepo.State;
        }

        public async Task Save(DataSeedObject value)
        {
            _dataSeedRepo.State = value;
            await _dataSeedRepo.WriteStateAsync();
        }
    }
}
