using System.Collections.Generic;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration
{
    public class SourceConfig
    {
        public Dictionary<string, DataSourceConfiguration> Configurations { get; set; }

        public DataSourceType DataSourceType { set; get; }
    }
}
