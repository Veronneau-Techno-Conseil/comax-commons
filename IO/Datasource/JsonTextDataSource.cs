using CommunAxiom.Commons.Client.IO.Configuration;
using CommunAxiom.Commons.Client.IO.Configuration.ValidatorFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Datasource
{
    public class JsonTextDataSource : IDataSource
    {
        const string NAME_FIELD = "FileName";

        private IEnumerable<FieldMetadata> _configurationFields = new FieldMetadata[0];
        public IEnumerable<FieldMetadata> ConfigurationFields
        {
            get
            {
                return _configurationFields;
            }
        }

        public SourceType SourceType 
        { 
            get
            {
                return Configuration.SourceType.JsonTextFile;
            }
        }

        public async IAsyncEnumerator<JObject> ReadData()
        {
            using (var fs = new FileStream(_configurationFields.First(x=>x.Name == NAME_FIELD).Parameter , FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
            {
                while (await jsonTextReader.ReadAsync())
                {
                    if (jsonTextReader.Depth == 0 && (jsonTextReader.TokenType == JsonToken.StartArray
                            || jsonTextReader.TokenType == JsonToken.EndArray))
                        continue;

                    var obj = await JObject.LoadAsync(jsonTextReader);
                    yield return obj;
                }
            }
        }

        public void Setup(IEnumerable<FieldMetadata>? fieldMetadatas = null)
        {
            List<FieldMetadata> list = new List<FieldMetadata>();
            if (fieldMetadatas != null)
            {
                list.AddRange(fieldMetadatas);
            }
            if(!list.Any(x=>x.Name == NAME_FIELD))
            {
                list.Add(Factory.Create(FieldType.TextFile, fm =>
                {
                    fm.Name = NAME_FIELD;
                    fm.FieldName = "fileName";
                }));
            }
            _configurationFields = list.ToArray();
        }

        public IEnumerable<ValidationError> ValidateDataSourceConfig()
        {
            foreach(var field in ConfigurationFields)
            {
                foreach(var item in field.Manager.ValidateConfig(field))
                {
                    yield return item;
                }
            }
        }
    }
}
