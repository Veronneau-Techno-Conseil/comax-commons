using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Portfolio
{
    public class PortfolioDetails
    {
        public const string PROJECT = "Project";
        public const string DATABASE = "Database";
        public string PortfolioID { get; set; }
        public string PortfolioName { get; set; }
        public IEnumerable<PortfolioAssociations> AssociatedPortfolios { get; set; }

    }
}
