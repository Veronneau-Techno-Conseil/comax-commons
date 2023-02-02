using Blazored.Toast.Services;
using Blazorise;
using CommunAxiom.Commons.Client.Contracts.Grains.Explorer;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.ClientUI.Shared.Extensions;
using CommunAxiom.Commons.ClientUI.Shared.Helper;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using CommunAxiom.Commons.Ingestion;
using CommunAxiom.Commons.Ingestion.DataSource;
using CommunAxiom.Commons.Ingestion.Ingestor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;

namespace ClientUI.Components.Explorer
{
    public partial class Explore
    {
        private bool IsLoading { get; set; }

        [Inject] public IExplorerViewmodel ViewModel { get; set; }
        [Inject] protected IStringLocalizer<Explore> _localizer { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }


        public IList<SharedPortfolio> Portfolios;



        protected override async Task OnInitializedAsync()
        {
            Portfolios = await ViewModel.List();
        }

        private void ExpandAll(IEnumerable<PortfolioTreeViewItem> nodes)
        {
            foreach (var node in nodes)
            {
                node.Children = node.Children;
                ExpandAll(node.Children);
            }
        }

        public List<FieldMetaData> FieldMetaDataList = new();

    }
}