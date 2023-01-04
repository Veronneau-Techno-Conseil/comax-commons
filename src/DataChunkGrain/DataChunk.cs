using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;
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
        private readonly IPersistentState<DataChunkState> _dataChunkState;
        private readonly DataChunkRepo _repo;

        public DataChunk([PersistentState("dataChunkGrain")] IPersistentState<DataChunkState> dataChunkState)
        {
            _dataChunkState = dataChunkState;
            _repo = new DataChunkRepo(dataChunkState);
        }

        public Task<DataChunkState> GetData()
        {
            return _repo.Fetch();
        }

        public Task SaveData(DataChunkState value)
        {
            return _repo.Save(value);
        }
    }
}
