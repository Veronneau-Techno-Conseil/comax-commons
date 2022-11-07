using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using System.Collections.Generic;

namespace CommunAxiom.Commons.Client.Contracts.IO
{
    public class SourceState
    {
        public Dictionary<string, DataSourceConfiguration> Configurations { get; set; }

        public DataSourceType DataSourceType { set; get; }

        public List<FieldMetaData> Fields { get; set; }
    }
}
