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
    public partial class PortfolioDetail
    {
        private bool IsLoading { get; set; }

        [Inject] public IExplorerViewmodel ViewModel { get; set; }
        [Inject] public IStringLocalizer<Explore> Localizer { get; set; }
        [Inject] private IToastService ToastService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string pfid { get; set; }

        private SharedPortfolioDetails _portfolioDetails;

        private IList<PortfolioModel> _items;

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrWhiteSpace(pfid))
                NavigationManager.NavigateTo("explore");

            _portfolioDetails = await this.ViewModel.LoadDetails(pfid);
            _items = _portfolioDetails.Entries.Portfolios.Select(x => new PortfolioModel { ID = x.ID, Name = x.Name, ParentId = x.ParentId, Type = x.Type }).ToList();
        }
    }
}