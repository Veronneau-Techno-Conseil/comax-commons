namespace CommunAxiom.Commons.Ingestion.Configuration
{
    public class SourceConfig
    {
        public Dictionary<string, DataSourceConfiguration> Configurations { get; set; }

        public DataSourceType DataSourceType { set; get; }
    }
}
