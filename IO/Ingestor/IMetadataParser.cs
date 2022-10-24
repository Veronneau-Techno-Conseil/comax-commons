using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public interface IMetadataParser
    {
        IEnumerable<FieldMetaData> ReadMetadata(string content);
    }
}
