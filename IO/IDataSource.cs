using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Client.IO.Configuration
{
    public interface IDataSource
    {
        SourceType SourceType { get; }

        IEnumerable<FieldMetadata> ConfigurationFields { get; }

        void Setup(IEnumerable<FieldMetadata>? fieldMetadatas = null);

        IEnumerable<ValidationError> ValidateDataSourceConfig();

        IAsyncEnumerator<JObject> ReadData();

    }
}
