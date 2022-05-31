using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Seeding
{
    public class DataSeed
    {
        public decimal Size { get; set; }
        public string Id { get; set; }
        public long TotalChunks { get; set; }
    }
}
