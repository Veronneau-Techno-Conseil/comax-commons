using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Seeding
{
    public class DataChunk
    {
        public int Index { get; set; }
        public string Source { get; set; }
        public string SourceId { get; set; }
        public byte[] Data { get; set; }
    }
}
