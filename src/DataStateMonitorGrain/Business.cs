using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Datasource;
using CommunAxiom.Commons.Client.Contracts.Ingestion;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Orleans;

namespace CommunAxiom.Commons.Client.Grains.DataStateMonitorGrain
{
    public class Business
    {
        private readonly IComaxGrainFactory _grainFactory;
        private readonly string _grainKey;
        private readonly IDataSourceFactory _dataSourceFactory;
        
        public Business(IComaxGrainFactory grainFactory, string grainKey, IDataSourceFactory dataSourceFactory)
        {
            _grainFactory = grainFactory;
            _grainKey = grainKey;
            _dataSourceFactory = dataSourceFactory;
        }
        
        public async Task Execute()
        {
            var datasource = _grainFactory.GetGrain<IDatasource>(_grainKey);
            var sourceState = await datasource.GetConfig();

            var dataSourceReader = _dataSourceFactory.Create(DataSourceType.File);
            dataSourceReader.Setup(new SourceConfig
            {
                Configurations = sourceState.Configurations,
                DataSourceType = sourceState.DataSourceType
            });
            
            var currentHashFile = await datasource.GetFileHash();
            var fileHash = dataSourceReader.CalculateHash();

            if (Encoding.Default.GetString(fileHash) != Encoding.Default.GetString(currentHashFile))
            {
                var ingestion = _grainFactory.GetGrain<IIngestion>(_grainKey);
                await ingestion.Run();
                await datasource.SetFileHash(fileHash);
            }
        }
    }
}