using CommunAxiom.Commons.Client.Contracts.Grains.Explorer;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using CommunAxiom.Commons.Shared;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels
{
    public class ExplorerViewModel : IExplorerViewmodel
    {
        private readonly HttpClient _httpClient;

        public ExplorerViewModel(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<SharedPortfolioDetails> LoadDetails(string portfolioUri)
        {
            var res = await _httpClient.GetAsync($"/api/Explore/details/{Uri.EscapeDataString(portfolioUri)}");
            res.EnsureSuccessStatusCode();
            var opRes = await res.ReadAsync<OperationResult<SharedPortfolioDetails>>();
            return opRes.Result;
        }

        public async Task<List<SharedPortfolio>> List()
        {
            var res = await _httpClient.GetAsync($"/api/Explore");
            res.EnsureSuccessStatusCode();
            var opRes = await res.ReadAsync<OperationResult<List<SharedPortfolio>>>();
            return opRes.Result;
        }
    }
}
