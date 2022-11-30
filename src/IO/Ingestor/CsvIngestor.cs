using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.IO;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public class CsvIngestor : IngestorBase, IIngestor
    {
        private IEnumerable<FieldMetaData> _fields;
        
        public void Configure(IEnumerable<FieldMetaData> fields)
        {
            _fields = fields;
        }

        public Task<IngestorResult> ParseAsync(Stream stream)
        {
            var ingestorResult = new IngestorResult();

            using var streamReader = new StreamReader(stream);
            var csvReader = new CsvReader(streamReader, ",");
            while (csvReader.Read())
            {
                var row = csvReader.RowAsJObject();
                
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
            
            return Task.FromResult(ingestorResult);
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