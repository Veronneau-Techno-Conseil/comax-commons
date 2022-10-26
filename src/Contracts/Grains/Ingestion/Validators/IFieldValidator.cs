using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion.Validators
{
    public interface IFieldValidator
    {
        string Tag { get; }
        string Parameter { get; set; }
        ValidationError Validate(FieldMetaData field, JObject row);
    }
}