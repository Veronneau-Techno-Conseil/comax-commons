using System;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion
{
    public class IngestionHistory
    {
        public DateTime CreateDateTime { get; set; }
        public bool IsSuccessful { get; set; }
        public Exception Exception { get; set; }
    }
}
