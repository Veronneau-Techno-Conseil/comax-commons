using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.DataChunkGrain
{
    public class DataChunkRepo
    {
        private readonly IPersistentState<DataChunkState> _state;
        public DataChunkRepo(IPersistentState<DataChunkState> state)
        {
            _state = state;
        }
        public async Task<DataChunkState> Fetch()
        {
            await _state.ReadStateAsync();
            return _state.State;
        }

        public async Task Save(DataChunkState value)
        {
            _state.State = value;
            await _state.WriteStateAsync();
        }
    }
}
