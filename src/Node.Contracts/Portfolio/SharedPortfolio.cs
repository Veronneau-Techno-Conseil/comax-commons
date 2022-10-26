using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio
{
    public class SharedPortfolio
    {
        public string OwnerId { get; set; }
        public string PortfolioId { get; set;}
        public Guid ItemId { get; set; }
        public string ItemType { get; set; }
    }
}
