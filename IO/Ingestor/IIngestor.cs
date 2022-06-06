using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Ingestor;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Injestor
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIngestor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        IngestorResult Parse(Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
        void Configure(IEnumerable<FieldMetaData> fields);
    }
}

