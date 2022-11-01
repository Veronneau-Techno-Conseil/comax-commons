using System;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class PortfolioTreeViewItem
	{
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public PortfolioType Type { get; set; }
        public IList<PortfolioTreeViewItem> Children { get; set; }
    }
}

