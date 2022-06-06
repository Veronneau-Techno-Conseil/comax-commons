namespace CommunAxiom.Commons.Ingestion.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class SourceConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, DataSourceConfiguration> Configurations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        DataSourceType DataSourceType { get; }
    }
}
