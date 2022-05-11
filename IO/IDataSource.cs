using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.IO.Configuration
{
    public interface IDataSource
    {
        SourceType SourceType { get; }
        IEnumerable<FieldMetadata> ConfigurationFields
        {
            get;
        }

        void Setup(IEnumerable<FieldMetadata>? fieldMetadatas = null);
        IEnumerable<ValidationError> ValidateDataSourceConfig();

        IAsyncEnumerator<Newtonsoft.Json.Linq.JObject> ReadData();

    }
}
