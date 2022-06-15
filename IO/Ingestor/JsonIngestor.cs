﻿using CommunAxiom.Commons.Ingestion.Attributes;
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    [IngestionType(IngestorType.JSON)]
    public class JsonIngestor : IngestorBase, IIngestor
    {
        private IEnumerable<FieldMetaData> _fieldMetaDatas;
        private readonly IFieldValidatorLookup _fieldValidatorLookup;

        public JsonIngestor(IFieldValidatorLookup fieldValidatorLookup)
        {
            _fieldValidatorLookup = fieldValidatorLookup;
        }

        public void Configure(IEnumerable<FieldMetaData> fieldMetaDatas)
        {
            _fieldMetaDatas = fieldMetaDatas;
        }

        public async Task<IngestorResult> ParseAsync(Stream stream)
        {
            IngestorResult ingestorResult = new IngestorResult();
            List<JObject> data = new List<JObject>();

            using (var streamReader = new StreamReader(stream))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                while (await jsonTextReader.ReadAsync())
                {
                    if (jsonTextReader.Depth == 0 && (jsonTextReader.TokenType == JsonToken.StartArray
                            || jsonTextReader.TokenType == JsonToken.EndArray))
                        continue;

                    var obj = await JObject.LoadAsync(jsonTextReader);
                    data.Add(obj);
                }
            }

            foreach (var item in data)
            {
                foreach (var result in Validate(item))
                {
                    if (result != null)
                    {
                        ingestorResult.Errors.Add((item, result));
                    }
                }

            }

            ingestorResult.results = data;
            return ingestorResult;
        }

        protected override IEnumerable<ValidationError> Validate(JObject data)
        {
            foreach (var fieldMetaData in _fieldMetaDatas)
            {
                foreach (var validate in fieldMetaData.Validators)
                {
                    var validator = _fieldValidatorLookup.Get(validate.Tag);
                    if (validator != null)
                    {
                        yield return validator.Validate(fieldMetaData, data);
                    }
                }
            }
        }
    }
}

