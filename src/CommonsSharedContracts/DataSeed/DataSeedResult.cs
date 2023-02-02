using CommunAxiom.Commons.CommonsShared.Contracts.DataSeed;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeedResult
    {
        public IList<DataSeedObject> DS { get; } = new List<DataSeedObject>();
        public DataSeedResult(DataSeedObject value)
        {
            DS.Add(value);
        }
    }
}
