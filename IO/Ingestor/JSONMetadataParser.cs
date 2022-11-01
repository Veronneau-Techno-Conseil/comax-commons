using System.Text.RegularExpressions;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using Newtonsoft.Json;

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

        private string FormatDisplayName(string name)
        {
            var n = Regex.Replace(name, @"(\[\d+\].)", string.Empty);
            return n;
        }

        private void SetProperty(JsonReader jsonReader)
        {
            var name = FormatName(jsonReader.Path);
            if (!metaDictionary.ContainsKey(name))
            {
                metaDictionary.Add(name, new FieldMetaData()
                {
                    FieldName = name,
                    DisplayName = FormatDisplayName(jsonReader.Path),
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
        public IEnumerable<FieldMetaData> ReadMetadata(string content)
        { 
            JsonReader jsonReader = new JsonTextReader(new StringReader(content));
            while (jsonReader.Read())
            {
                switch (jsonReader.TokenType)
                {
                    case JsonToken.StartObject:
                    case JsonToken.PropertyName:
                    case JsonToken.StartArray:
                    case JsonToken.EndArray:
                    case JsonToken.Comment:
                    case JsonToken.EndObject:
                    case JsonToken.Undefined:
                        continue;
                    default:
                        SetProperty(jsonReader);
                        break;
                }
            }

            return metaDictionary.Values;
        }
    }
}
