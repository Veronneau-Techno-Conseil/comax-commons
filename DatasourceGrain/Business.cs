using System.Collections.Generic;
using CommunAxiom.Commons.Client.Contracts.IO;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Grains.DatasourceGrain
{
    public class Business
    {
        private readonly Repo _repo;

        public Business(Repo repo)
        {
            _repo = repo;
        }

        public async Task SetConfig(DataSourceType dataSourceType,
            Dictionary<string, DataSourceConfiguration> configurations)
        {
            //todo should be validate all configurations here? 
            await _repo.SetConfig(dataSourceType, configurations);
        }

        public async Task SetFieldMetaData(List<FieldMetaData> fieldMetaDatas)
        {
            await _repo.SetFieldMetaData(fieldMetaDatas);
        }

        public async Task<List<FieldMetaData>> GetFieldMetaData()
        {
            return await _repo.GetFieldMetaData();
        }

        public async Task<SourceState> ReadConfig()
        {
            return await _repo.ReadConfig();
        }

        public Task DeleteConfig()
        {
            return _repo.DeleteConfig();
        }
    }
}