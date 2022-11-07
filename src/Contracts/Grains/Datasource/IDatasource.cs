using System.Collections.Generic;
using CommunAxiom.Commons.Client.Contracts.IO;
using Orleans;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Client.Contracts.Datasource
{
    public interface IDatasource : IGrainWithStringKey
    {
        Task<SourceState> GetConfig();
        Task SetConfig(DataSourceType dataSourceType, Dictionary<string, DataSourceConfiguration> configurations);
        Task SetFieldMetaData(List<FieldMetaData> fieldMetaDatas);
        Task<SourceState> GetSourceState();
        Task DeleteConfig();
    }
}