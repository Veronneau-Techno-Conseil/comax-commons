using CommunAxiom.Commons.Client.IO.Configuration;
using CommunAxiom.Commons.Client.IO.Configuration.ValidatorFactory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Datasource
{
    public class CsvTextDataSource : IDataSource
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
            yield break;
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
                list.Add(Factory.Create(FieldType.Text, fm =>
                {
                    fm.Mandatory = true;
                    fm.Name = NAME_FIELD;
                    fm.FieldName = "delimiter";
                }));
                list.Add(Factory.Create(FieldType.Text, fm =>
                {
                    fm.Name = NAME_FIELD;
                    fm.FieldName = "columnWrapper";
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
