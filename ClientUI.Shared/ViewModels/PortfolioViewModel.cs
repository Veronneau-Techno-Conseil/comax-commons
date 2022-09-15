using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.Extensions;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels
{
    public class PortfolioViewModel : IPortfolioViewModel
    {
        private const string RootKey = "79186558-5e84-4fd9-904b-58d74ba582af";

        private readonly HttpClient _httpClient;
        public PortfolioViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Guid ID { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }

        public async Task CreatePortfolio(PortfolioModel portfolio)
        {
            await _httpClient.PostAsJsonAsync("/api/Portfolio/Create", portfolio);
        }

        public async Task<IEnumerable<PortfolioTreeViewItem>> GetPortfolios()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Portfolio/GetAll");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var errorMessage = httpResponseMessage.ReasonPhrase;
                Console.WriteLine($"There was an error! {errorMessage}");
                return null;
            }
            else
            {
                if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                {
                    Console.WriteLine("Portfolio List is defined but has no content");

                    var list = new List<Portfolio>();

                    list.Add(new Portfolio { ID = new Guid(RootKey), Name = "Root", Type = "Folder", ParentId = Guid.Empty });

                    return list?.ToTree(o => o.ID, o => o.ParentId);
                }
                var response = await httpResponseMessage.Content.ReadFromJsonAsync<List<Portfolio>>();


                response.Insert(0, new Portfolio { ID = new Guid(RootKey), Name = "Root", Type="Folder", ParentId = Guid.Empty });
                
                
                return response?.ToTree(o => o.ID, o => o.ParentId);

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
                return await httpResponseMessage.Content.ReadFromJsonAsync<bool>();
            }
        }

        public string GetIcon(PortfolioType portfolioType)
        {
            var iconName = portfolioType.GetDisplayDescription();
            return $"_content/ClientUI.Components/icons/{iconName}.png";
        }

        public List<string> GetDatasources()
        {
            return new List<string> { "JSON File", "CSV file" };
        }

        public List<string> GetPortfolioTypes()
        {
            return new List<string>() {
                PortfolioType.Folder.GetDisplayDescription(),
                PortfolioType.Project.GetDisplayDescription(),
                PortfolioType.Dataset.GetDisplayDescription()
            };
        }
    }
}
