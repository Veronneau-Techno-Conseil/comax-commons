using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Portfolio
{
    public class PortfoliosList
    {
        public IEnumerable<Portfolio> Portfolios { get; set; }
    }
}
