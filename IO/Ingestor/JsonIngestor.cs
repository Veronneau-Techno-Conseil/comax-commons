using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Ingestor;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Injestor
{
    public class JsonIngestor : IngestorBase, IIngestor
    {

        public void Configure(IEnumerable<FieldMetaData> fields)
        {
            throw new NotImplementedException();
        }

        public IngestorResult Parse(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ValidationError> Validate(JObject data)
        {
            throw new NotImplementedException();
        }
    }
}

