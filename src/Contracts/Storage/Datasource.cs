using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Storage
{
    public class Datasource
    {
        public Guid Id { get; set; }
        public Guid Owner { get; set; }
        public bool Indexed { get; set; }
        public string Name { get; set; }
        public SourceType SourceType { get; set; }
        public DateTime CreateionDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
