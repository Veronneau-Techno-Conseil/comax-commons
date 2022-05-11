using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Configuration
{
    public interface IFieldValidator
    {
        IEnumerable<ValidationError> ValidateConfig(FieldMetadata field);
        IEnumerable<ValidationError> ValidateData(FieldMetadata field, JObject o);
    }
}