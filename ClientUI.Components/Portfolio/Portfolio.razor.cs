using Blazorise;
using CommunAxiom.Commons.ClientUI.Shared.Extensions;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace ClientUI.Components.Portfolio
{
    public partial class Portfolio
    {
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IPortfolioViewModel PortfolioViewModel { get; set; }
        [Inject] private IStringLocalizer<Portfolio> Localizer { get; set; }

        private string _filterFile;
        private string _selectedTab = "tab1";
        private PortfolioTreeViewItem selectedPortfolio = new PortfolioTreeViewItem();
        private IList<PortfolioModel> _portfolioList;
        private IEnumerable<PortfolioTreeViewItem> _portfolioListResult;
        private IList<PortfolioTreeViewItem> _expandedPortfolio = new List<PortfolioTreeViewItem>();
        private PortfolioModel _modalViewModel = new PortfolioModel();

        private List<string> _types;

        // reference to the modal component
        private Modal? _modalRefefence;
        private bool _cancelClose;
        private PortfolioTreeViewItem _selectedPortfolio;
        private PortfolioTreeViewItem SelectedPortfolio { get; set; }
        private bool HideCard { get; set; } = true;
        
        private void OnChangeType(string value)
        {
            _modalViewModel.Type = value;
        }
        
        private Task OnSelectedTab(string name)
        {
            _selectedTab = name;
            return Task.CompletedTask;
        }

        private void OnHideCard(PortfolioTreeViewItem item)
        {
            HideCard = item.Type == PortfolioType.Project || item.Type == PortfolioType.Folder;
        }

        private Task ShowModal(PortfolioTreeViewItem currentPortfolio)
        {
            this.SelectedPortfolio = currentPortfolio;
            return _modalRefefence.Show();
        }

        private void AddToTree(PortfolioTreeViewItem parent, PortfolioTreeViewItem newItem)
        {
            if (parent.Children == null)
            {
                parent.Children = new List<PortfolioTreeViewItem>();
            }

            parent.Children.Add(newItem);
        }

        private async Task onModalSave()
        {
            // TODO: check the name of portfolio
            //if (string.IsNullOrEmpty(modalViewModel.Name))
            //{
            //    toastService.ShowError(_localizer["NameIsRequired"], "ERROR");
            //    return;
            //}

            var newPortfolio = new PortfolioModel
            {
                ID = Guid.NewGuid(),
                Name = _modalViewModel.Name,
                ParentId = this.SelectedPortfolio.Id,
                Type = this._modalViewModel.Type
            };

            await PortfolioViewModel.CreatePortfolio(newPortfolio);

            AddToTree(this.SelectedPortfolio, new PortfolioTreeViewItem
            {
                Id = newPortfolio.ID,
                Text = newPortfolio.Name,
                ParentId = newPortfolio.ParentId,
                Type = Enum.Parse<PortfolioType>(newPortfolio.Type)
            });


            ClearModel();
            _cancelClose = false;
            await _modalRefefence.Hide();
        }

        private async Task onModalCancel()
        {
            ClearModel();
            _cancelClose = true;
            await _modalRefefence.Hide();
        }


        private void ClearModel()
        {
            _modalViewModel.ID = Guid.Empty;
            _modalViewModel.ParentId = Guid.Empty;
            _modalViewModel.Name = string.Empty;
            this._modalViewModel.Type = this._types[0];
        }

        protected override async Task OnInitializedAsync()
        {
            _types = PortfolioViewModel.GetPortfolioTypes();
            _modalViewModel.Type = _types[0];
            _portfolioList = await PortfolioViewModel.GetPortfolios();
            _portfolioListResult = _portfolioList?.ToTree(o => o.ID, o => o.ParentId);
            if (_portfolioListResult != null) ExpandAll(_portfolioListResult);
        }

        private void ExpandAll(IEnumerable<PortfolioTreeViewItem> nodes)
        {
            foreach (var node in nodes)
            {
                node.Children = node.Children;
                ExpandAll(node.Children);
            }

            ((List<PortfolioTreeViewItem>)_expandedPortfolio).AddRange(nodes);
        }

        private void GoToCreatePortfolio()
        {
            NavigationManager.NavigateTo("/create_portfolio");
        }

        
    }
}