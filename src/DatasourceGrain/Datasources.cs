using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.IO;
using Orleans;
using Orleans.Runtime;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Grains.DatasourceGrain
{
    public class Datasources : Grain, IDatasource
    {
        private readonly Business _business;

        public Datasources(
            [PersistentState("DataSource")] IPersistentState<SourceState> state,
            [PersistentState("FileHash")] IPersistentState<byte[]> hashState)
        {
            _business = new Business(new Repo(state, hashState));
        }

        public Task<SourceState> GetConfig()
        {
            return _business.ReadConfig();
        }

        public async Task SetConfig(DataSourceType dataSourceType,
            Dictionary<string, DataSourceConfiguration> configurations)
        {
            await _business.SetConfig(dataSourceType, configurations);
        }

        public async Task SetFieldMetaData(List<FieldMetaData> fieldMetaDatas)
        {
            await _business.SetFieldMetaData(fieldMetaDatas);
        }

        public async Task<SourceState> GetSourceState()
        {
            return await _business.GetSourceState();
        }

        public async Task DeleteConfig()
        {
            await _business.DeleteConfig();
        }

        public async Task SetFileHash(byte[] hash)
        {
            await _business.SetFileHash(hash);
        }

        public Task<byte[]> GetFileHash()
        {
            return _business.GetFileHash();
        }
        
    }
}