using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public class JSONMetadataParser : IMetadataParser
    {
        Regex replaceIndex = new Regex(@"(\[\d+\])");
        Dictionary<string, FieldMetaData> metaDictionary = new Dictionary<string, FieldMetaData>();

        private string FormatName(string name)
        {
            var n = replaceIndex.Replace(name, "[]");
            return n;
        }

        public IEnumerable<FieldMetaData> ReadMetadata(string content)
        {
            Newtonsoft.Json.JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(new StringReader(content));
            while (jsonReader.Read())
            {
                switch (jsonReader.TokenType)
                {
                    case Newtonsoft.Json.JsonToken.StartObject:
                    case Newtonsoft.Json.JsonToken.PropertyName:
                    case Newtonsoft.Json.JsonToken.StartArray:
                    case Newtonsoft.Json.JsonToken.EndArray:
                    case Newtonsoft.Json.JsonToken.Comment:
                    case Newtonsoft.Json.JsonToken.EndObject:
                    case Newtonsoft.Json.JsonToken.Undefined:
                        continue;
                    default:
                        SetProperty(jsonReader);
                        break;
                }
            }
            
            return metaDictionary.Values;
        }

        private void SetProperty(JsonReader jsonReader)
        {
            var name = FormatName(jsonReader.Path);
            if (!metaDictionary.ContainsKey(name))
            {
                metaDictionary.Add(name, new FieldMetaData()
                {
                    FieldName = name,
                    DisplayName = name,
                    Index = metaDictionary.Keys.Count
                });
            }
            var entry = metaDictionary[name];
            SetType(jsonReader, entry);

        }

        private void SetType(JsonReader jsonReader, FieldMetaData entry)
        {
            var fieldType = Convert(jsonReader.TokenType);
            if(entry.FieldType == FieldType.Undefined)
            {
                entry.FieldType = fieldType;
            }
            if(entry.FieldType != FieldType.Undefined && fieldType != entry.FieldType)
            {
                switch (entry.FieldType)
                {
                    case FieldType.Date:
                        if (fieldType == FieldType.Text)
                            entry.FieldType = FieldType.Text;
                        else
                            entry.FieldType = FieldType.Object;
                        break;
                    case FieldType.Integer:
                        if(fieldType == FieldType.Decimal)
                            entry.FieldType = FieldType.Decimal;
                        else
                            entry.FieldType = FieldType.Object;
                        break;
                    default:
                        entry.FieldType = FieldType.Object;
                        break;
                }
            }
        }

        private FieldType Convert(JsonToken tokenType)
        {
            switch (tokenType)
            {
                case JsonToken.Boolean:
                    return FieldType.Boolean;
                case JsonToken.Bytes:
                    return FieldType.Text;
                case JsonToken.Comment:
                    return FieldType.Undefined;
                case JsonToken.Date:
                    return FieldType.Date;
                case JsonToken.EndArray:
                    return FieldType.Undefined;
                case JsonToken.EndConstructor:
                    return FieldType.Undefined;
                case JsonToken.EndObject:
                    return FieldType.Undefined;
                case JsonToken.Float:
                    return FieldType.Decimal;
                case JsonToken.Integer:
                    return FieldType.Integer;
                case JsonToken.None:
                    return FieldType.Undefined;
                case JsonToken.Null:
                    return FieldType.Undefined;
                case JsonToken.PropertyName:
                    return FieldType.Undefined;
                case JsonToken.Raw:
                    return FieldType.Undefined;
                case JsonToken.StartArray:
                    return FieldType.Undefined;
                case JsonToken.StartConstructor:
                    return FieldType.Undefined;
                case JsonToken.StartObject:
                    return FieldType.Undefined;
                case JsonToken.String:
                    return FieldType.Text;
                case JsonToken.Undefined:
                    return FieldType.Undefined;
                default:
                    return FieldType.Undefined;
            }
        }
    }
}
