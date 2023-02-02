using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio.Models
{
    public class DatasourceDetails
    {
        public List<FieldMetaData> Fields { get; set; }
        public byte[] Sample { get; set; }
    }
}
