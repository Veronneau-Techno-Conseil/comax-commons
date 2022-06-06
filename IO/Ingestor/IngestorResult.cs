using System;
using CommunAxiom.Commons.Ingestion.Configuration;
using Newtonsoft.Json.Linq;

namespace CommunAxiom.Commons.Ingestion.Ingestor
{
    /// <summary>
    /// 
    /// </summary>
    public class IngestorResult
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<(JObject, IEnumerable<ValidationError>)> Errors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<JObject> results { get; set; }
    }
}

