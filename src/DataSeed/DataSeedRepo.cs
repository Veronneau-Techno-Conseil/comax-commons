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
        private readonly IPersistentState<DataSeedState> _dataSeedRepo;
        public DataSeedRepo(IPersistentState<DataSeedState> dataSeedRepo)
        {
            this._dataSeedRepo = dataSeedRepo;
        }
        public async Task<DataSeedState> Fetch()
        {
            await _dataSeedRepo.ReadStateAsync();
            return _dataSeedRepo.State;
        }

        public async Task Save(DataSeedState value)
        {
            _dataSeedRepo.State = value;
            await _dataSeedRepo.WriteStateAsync();
        }
    }
}
