using System;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class PortfolioTreeViewItem
	{
        public Guid Id { get; set; }
        public string Text { get; set; }
        public PortfolioType Type { get; set; }
        public IEnumerable<PortfolioTreeViewItem> Children { get; set; }
    }
}

