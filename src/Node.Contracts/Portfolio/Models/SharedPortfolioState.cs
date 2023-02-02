using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio.Models
{
    public class SharedPortfolioState
    {
        public string OwnerId { get; set; }
        public List<PortfolioItem> PortfolioItems { get; set; }
    }
}
