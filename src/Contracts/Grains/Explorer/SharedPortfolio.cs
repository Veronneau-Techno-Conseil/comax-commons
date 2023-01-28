using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Explorer
{
    public class SharedPortfolio
    {
        public string OwnerUsername { get; set; }
        public string OwnerUri { get; set; }
        public string PortfolioUri { get; set; }
    }
}
