using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataChunk
{
    public class DataChunkObject
    {
        public Guid Id { get; set; }
        public JObject Data { get; set; }
        public Guid IdDataSeed { get; set; }
    }
}
