using CommunAxiom.Commons.Client.Contracts.Grains.Explorer;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class SharedPortfolioDetails
    {
        public SharedPortfolio SharedPortfolio { get; set; }
        public PortfoliosList Entries { get; set; }
    }
}
