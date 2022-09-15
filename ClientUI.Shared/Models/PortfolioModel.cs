using System;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
	public class PortfolioModel
	{
        public Guid ID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
    }
}

