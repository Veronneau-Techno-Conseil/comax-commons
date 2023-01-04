using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataChunk
{
    public class DataChunk
    {
        public int Index { get; set; }
        public JObject Data { get; set; }
        public Guid DataSeedId { get; set; }
    }
}
