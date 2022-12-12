using Newtonsoft.Json.Linq;
using System.Collections.Generic;


namespace Comax.Commons.Orchestrator.DataSeedGrain
{
    public class DataSeedResult
    {
        public IList<JObject> Rows { get; } = new List<JObject>();
        public DataSeedResult(JObject row)
        {
            Rows.Add(row);
        }
    }
}
