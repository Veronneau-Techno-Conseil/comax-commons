using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Validators
{
    public interface IFieldValidator
    {
        string Tag { get; }

        string Parameter { get; set; }

        ValidationError Validate(FieldMetaData fieldMetaData, JObject obj);
    }
}