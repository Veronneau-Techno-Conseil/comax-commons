using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public class IngestorResult
    {
        public IList<(JObject, ValidationError)> Errors { get; } = new List<(JObject, ValidationError)>();

        public IList<JObject> Rows { get; } = new List<JObject>();

    }
}

