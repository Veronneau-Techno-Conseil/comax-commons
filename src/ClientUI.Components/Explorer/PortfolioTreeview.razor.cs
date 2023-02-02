using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.Extensions;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Components.Explorer
{
    public partial class PortfolioTreeview
    {

        private IList<PortfolioModel> _portfolios;

        [Parameter]
        public IList<PortfolioModel> PortfolioList
        {
            get
            {
                return _portfolios;
            }
            set
            {
                _portfolios = value;
                PortfolioListResult = _portfolios?.ToTree(o => o.ID, o => o.ParentId);
                if (PortfolioListResult != null) ExpandAll(PortfolioListResult);
            }
        }
        public IEnumerable<PortfolioTreeViewItem> PortfolioListResult;
        public IList<PortfolioTreeViewItem> ExpandedPortfolio = new List<PortfolioTreeViewItem>();
        private PortfolioTreeViewItem SelectedPortfolio { get; set; } = new PortfolioTreeViewItem();

        public void ExpandAll(IEnumerable<PortfolioTreeViewItem> nodes)
        {
            foreach (var node in nodes)
            {
                node.Children = node.Children;
                ExpandAll(node.Children);
            }

           ((List<PortfolioTreeViewItem>)ExpandedPortfolio).AddRange(nodes);
        }

        protected void ShowCard(PortfolioTreeViewItem context)
        {

        }


        public string GetIcon(PortfolioType portfolioType)
        {
            var iconName = portfolioType.GetDisplayDescription();
            return $"_content/ClientUI.Components/icons/{iconName}.png";
        }
    }
}
