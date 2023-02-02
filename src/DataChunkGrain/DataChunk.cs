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
        private readonly IPersistentState<DataChunkObject> _dataChunkState;
        private readonly DataChunkRepo _repo;

        public DataChunk([PersistentState("dataChunkGrain")] IPersistentState<DataChunkObject> dataChunkState)
        {
            _dataChunkState = dataChunkState;
            _repo = new DataChunkRepo(dataChunkState);
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
