using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public abstract class IngestorBase
    {
        protected abstract IEnumerable<ValidationError> Validate(JObject row);
    }
}

