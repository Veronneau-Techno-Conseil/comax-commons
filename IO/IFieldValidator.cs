using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Client.IO.Configuration
{
    public interface IFieldValidator
    {
        IEnumerable<ValidationError> ValidateConfig(FieldMetadata field);
        IEnumerable<ValidationError> ValidateData(FieldMetadata field, JObject o);
    }
}