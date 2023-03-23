using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataChunk
{
    public class DataChunkObject
    {
        public IdDataChunk IdDataChunk { get; set; }
        public JObject Data { get; set; }
    }
    public class IdDataChunk
    {
        public string Id { get; set; }
        public Guid IdDataSeed { get; set; }
    }
    
}
