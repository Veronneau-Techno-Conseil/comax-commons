using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Portfolio
{
    public class Portfolio
    {
        public Guid ID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
    }
}
