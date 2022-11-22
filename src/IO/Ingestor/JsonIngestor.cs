using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    [IngestionType(IngestorType.JSON)]
    public class JsonIngestor : IngestorBase, IIngestor
    {
        private IEnumerable<FieldMetaData> _fields;

        public void Configure(IEnumerable<FieldMetaData> fields)
        {
            _fields = fields;
        }

        public async Task<IngestorResult> ParseAsync(Stream stream)
        {
            var ingestorResult = new IngestorResult();

            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            while (await jsonTextReader.ReadAsync())
            {
                if (jsonTextReader.Depth == 0 && (jsonTextReader.TokenType == JsonToken.StartArray ||
                                                  jsonTextReader.TokenType == JsonToken.EndArray)) continue;
                var row = await JObject.LoadAsync(jsonTextReader);

                foreach (var result in Validate(row))
                {
                    if (result != null)
                    {
                        ingestorResult.Errors.Add((row, result));
                    }
                    else
                    {
                        ingestorResult.Rows.Add(row);
                    }
                }
            }

            return ingestorResult;
        }

        protected override IEnumerable<ValidationError> Validate(JObject row)
        {
            foreach (var field in _fields)
            {
                foreach (var validate in field.Validators)
                {
                    yield return validate.Validate(field, row);
                }
            }
        }
    }
}