using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataChunk
{
    public class DataChunkResult
    {
        public IList<DataChunkObject> DC { get; } = new List<DataChunkObject>();
        public DataChunkResult(DataChunkObject value)
        {
            DC.Add(value);
        }
    }
}
