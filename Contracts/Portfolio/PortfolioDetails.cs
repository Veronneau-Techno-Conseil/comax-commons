using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Portfolio
{
    public class PortfolioDetails
    {
        public const string PROJECT = "Project";
        public const string DATABASE = "Database";
        public string ID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        //public IEnumerable<PortfolioAssociations> AssociatedPortfolios { get; set; }
    }
}
