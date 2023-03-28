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
        private readonly IPersistentState<DataIndex> _dataIndexRepo;
        private bool _isRead = false;

        public async Task EnsureRead()
        {
            if (!_isRead)
            {
                await _dataIndexRepo.ReadStateAsync();
                _isRead = true;
            }
        }

        public DataSeedRepo(IPersistentState<DataIndex> dataIndexRepo)
        {
            this._dataIndexRepo = dataIndexRepo;
        }
        public async Task<DataIndex> Fetch()
        {
            await EnsureRead();
            return _dataIndexRepo.State;
        }

        public async Task Save(DataIndex value)
        {
            await EnsureRead();
            _dataIndexRepo.State = value;
            await _dataIndexRepo.WriteStateAsync();
        }
    }
}
