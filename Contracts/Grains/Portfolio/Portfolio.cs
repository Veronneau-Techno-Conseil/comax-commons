using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Portfolio
{
    public class Portfolio
    {
        public string ID { get; set; }
        public string TheType { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
    }
}
