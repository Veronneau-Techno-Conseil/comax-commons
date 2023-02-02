using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Portfolio.Models
{
    public class PortfolioItem
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string PortfolioUri { get; set; }
        public string Uri { get; set; }
        public string Name { get; set; }
        public ItemTypes ItemType { get; set; }
        public string Description { get; set; }
    }
}
