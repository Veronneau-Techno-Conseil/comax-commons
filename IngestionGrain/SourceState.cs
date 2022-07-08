using CommunAxiom.Commons.Ingestion.Configuration;
using System.Collections.Generic;

namespace CommunAxiom.Commons.Client.Grains.IngestionGrain
{
    // HACK: remove this class into Contract project.
    public class SourceState
    {
        public Dictionary<string, DataSourceConfiguration> Configurations { get; set; }

        public DataSourceType DataSourceType { set; get; }

        public List<FieldMetaData> Fields { get; set; }
    }
}
