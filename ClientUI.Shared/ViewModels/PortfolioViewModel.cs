//using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels
{
    public class PortfolioViewModel : IPortfolioViewModel
    {
        private readonly HttpClient _httpClient;
        public PortfolioViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            PROJECT = "Project";
            DATABASE = "Database";
        }

        public string PROJECT { get; }
        public string DATABASE { get; }
        public string ID { get; set; }
        [Required]
        public string TheType { get; set; }
        [Required]
        public string Name { get; set; }
        public string ParentId { get; set; }
        public Models.Portfolio portfolio { get; set; }
        public List<Models.Portfolio> Portfolios { get; set; }

        public async Task CreatePortfolio(Portfolio portfolio)
        {
            await _httpClient.PostAsJsonAsync("/api/Portfolio/Create", portfolio);
        }

        public async Task<List<Portfolio>?> GetPortfolios()
        {
            HttpResponseMessage? httpResponseMessage;
            httpResponseMessage = await _httpClient.GetAsync("/api/Portfolio/GetAll");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var errorMessage = httpResponseMessage.ReasonPhrase;
                Console.WriteLine($"There was an error! {errorMessage}");
                return null;
            }
            else
            {
                if ((int)httpResponseMessage.StatusCode == 204)
                {
                    Console.WriteLine("Portfolio List is defined but has no content");
                    return null;
                }
                return httpResponseMessage.Content.ReadFromJsonAsync<List<Portfolio>>().Result;
            }
        }

        public async Task<bool> CheckIfUnique(string name)
        {
            HttpResponseMessage? httpResponseMessage;
            httpResponseMessage = await _httpClient.GetAsync("/api/Portfolio/CheckName/" + name);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var errorMessage = httpResponseMessage.ReasonPhrase;
                Console.WriteLine($"There was an error! {errorMessage}");
                return false;
            }
            else
            {
                return httpResponseMessage.Content.ReadFromJsonAsync<bool>().Result;
            }
        }
    }
}
