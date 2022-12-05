using CommunAxiom.Commons.Client.Contracts.IO;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Grains.DatasourceGrain
{
    public class Repo
    {
        private readonly IPersistentState<SourceState> _dataSourceState;
        private readonly IPersistentState<byte[]> _hashState;
        
        public Repo(IPersistentState<SourceState> dataSourceState, IPersistentState<byte[]> hashState)
        {
            _dataSourceState = dataSourceState;
            _hashState = hashState;
        }

        public async Task<SourceState> ReadConfig()
        {
            await _dataSourceState.ReadStateAsync();
            return _dataSourceState.State;
        }

        public async Task SetConfig(DataSourceType dataSourceType, Dictionary<string, DataSourceConfiguration> configurations)
        {
            _dataSourceState.State.DataSourceType = dataSourceType;
            _dataSourceState.State.Configurations = configurations;
            await _dataSourceState.WriteStateAsync();
        }

        public async Task SetFieldMetaData(List<FieldMetaData> fieldMetaDatas)
        {
            _dataSourceState.State.Fields = fieldMetaDatas;
            await _dataSourceState.WriteStateAsync();
        }

        public Task<SourceState> GetSourceState()
        {
            return Task.FromResult(_dataSourceState.State);
        }

        public async Task DeleteConfig()
        {
            await _dataSourceState.ClearStateAsync();
        }

        public async Task SetFileHash(byte[] hash)
        {
            _hashState.State = hash;
            await _hashState.WriteStateAsync();
        }
        
        public async Task<byte[]> GetFileHash()
        {
            await _hashState.ReadStateAsync();
            return _hashState.State;
        }
    }
}