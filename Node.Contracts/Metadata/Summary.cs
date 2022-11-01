using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Metadata
{
    public class Summary
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public MetadataType MetadataType { get;set;}
        public List<DetailId> DetailIds { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
