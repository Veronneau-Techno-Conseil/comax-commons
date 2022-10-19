using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    public interface IMetadataParser
    {
        IEnumerable<FieldMetaData> ReadMetadata(string content);
    }
}
