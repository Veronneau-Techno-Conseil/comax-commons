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
        private readonly IPersistentState<DataChunkObject> _state;
        public DataChunkRepo(IPersistentState<DataChunkObject> state)
        {
            _state = state;
        }
        public async Task<DataChunkObject> Fetch()
        {
            await _state.ReadStateAsync();
            return _state.State;
        }

        public async Task Save(DataChunkObject value)
        {
            _state.State = value;
            await _state.WriteStateAsync();
        }
    }
}
