using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Portfolio
{
    public class PortfoliosList
    {
        public string OwnerUri { get; set; }
        public List<PortfolioItem> Portfolios { get; set; } = new List<PortfolioItem> { };
    }
}
