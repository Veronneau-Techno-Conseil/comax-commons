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
        private readonly IPersistentState<IdDataChunk> _stateId;
        private readonly IPersistentState<JObject> _stateData;
        public DataChunkRepo(IPersistentState<IdDataChunk> stateId, IPersistentState<JObject> stateData)
        {
            _stateId = stateId;
            _stateData = stateData;
        }
        public async Task<DataChunkObject> Fetch()
        {
            await _stateId.ReadStateAsync();
            await _stateData.ReadStateAsync();
            var state = new DataChunkObject { IdDataChunk = _stateId.State, Data = _stateData.State };
            return state;
        }

        public async Task Save(DataChunkObject value)
        {
            _stateId.State = value.IdDataChunk;
            _stateData.State = value.Data;
            await _stateId.WriteStateAsync();
            await _stateData.WriteStateAsync();
        }
    }
}
