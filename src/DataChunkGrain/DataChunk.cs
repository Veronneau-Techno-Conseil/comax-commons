using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;
using CommunAxiom.Commons.Orleans;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.DataChunkGrain
{
    public class DataChunk: Grain, IDataChunk
    {
        private readonly IPersistentState<IdDataChunk> _dataChunkIdState;
        private readonly IPersistentState<JObject> _dataChunkDataState;
        private readonly DataChunkRepo _repo;

        public DataChunk([PersistentState("dataChunkGrain_dataStore", OrleansConstants.Storage.JObjectStore)] IPersistentState<JObject> dataChunkData, [PersistentState("dataChunkGrain_idStore")] IPersistentState<IdDataChunk> dataChunkId)
        {
            _dataChunkIdState = dataChunkId;
            _dataChunkDataState = dataChunkData;
            _repo = new DataChunkRepo(_dataChunkIdState, _dataChunkDataState);
        }

        public Task<DataChunkObject> GetData()
        {
            return _repo.Fetch();
        }

        public Task SaveData(DataChunkObject value)
        {
            return _repo.Save(value);
        }
    }
}
