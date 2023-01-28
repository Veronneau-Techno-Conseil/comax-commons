using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Portfolio
{
    public class PortfolioItem
    {
        public Guid ID { get; set; }
        public PortfolioType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ParentId { get; set; }
        public string OwnerUri { get; set; }
        public string Uri { get; set; }
    }
}
