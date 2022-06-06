using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;

namespace CommunAxiom.Commons.Ingestion
{
    /// <summary>
    /// 
    /// </summary>
    public static class Importer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IngestorResult Import(DataSourceType sourceType, DataSourceConfiguration config)
        {
            var dataSourceReader = SourceFactory.Create(sourceType);
            var stream = dataSourceReader.ReadData();
            var ingestor = IngestorFactory.Create(dataSourceReader.IngestionType);
            return ingestor.Parse(stream);
        }
    }
}

