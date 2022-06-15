using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public class IngestorResult
    {
        public IList<(JObject, ValidationError)> Errors { get; set; }

        public IList<JObject> results { get; set; }
    }
}

