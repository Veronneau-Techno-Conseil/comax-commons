
using System.Collections.Generic;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio.Models
{
    public class FieldMetaData
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
        public FieldType FieldType { get; set; }
        public int? Index { get; set; }
        public string Description { get; set; }
        public string Configuration { get; set; }
    }
}
