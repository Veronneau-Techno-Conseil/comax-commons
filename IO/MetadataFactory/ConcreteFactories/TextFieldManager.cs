using CommunAxiom.Commons.Client.IO.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.MetadataFactory.ConcreteFactories
{
    public class TextFieldManager : IFieldManager
    {
        public static IFieldManager Instance { get; } = new TextFieldManager();
        private TextFieldManager()
        {

        }
        public void Configure(FieldMetadata field)
        {
            field.FieldType = FieldType.Text;
            field.Manager = this;
        }

        public IEnumerable<ValidationError> ValidateConfig(FieldMetadata field)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ValidationError> ValidateData(FieldMetadata field, JObject o)
        {
            throw new NotImplementedException();
        }
    }
}
