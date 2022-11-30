using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces;
using Radzen.Blazor;
using System.Net;
using System.Net.Http.Json;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.IO;

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

        public async Task CreatePortfolio(PortfolioModel portfolio)
        {
            await _httpClient.PostAsJsonAsync("/api/Portfolio/Create", portfolio);
        }

        public async Task<IList<PortfolioModel>> GetPortfolios()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Portfolio/GetAll");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return null;
            }

            if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
            {
                var list = new List<PortfolioModel>();

                list.Add(new PortfolioModel
                {
                    ID = new Guid(RootKey),
                    Name = "Root",
                    Type = "Folder",
                    ParentId = Guid.Empty
                });

                return list;
            }

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<List<PortfolioModel>>();
            
            response.Insert(0, new PortfolioModel
            {
                ID = new Guid(RootKey),
                Name = "Root",
                Type = "Folder",
                ParentId = Guid.Empty
            });
            
            return response;
        }

        public async Task<bool> CheckIfUnique(string name)
        {
            var httpResponseMessage = await _httpClient.GetAsync("/api/Portfolio/CheckName/" + name);
            
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return false;
            }

            return await httpResponseMessage.Content.ReadFromJsonAsync<bool>();
        }

        public string GetIcon(PortfolioType portfolioType)
        {
            var iconName = portfolioType.GetDisplayDescription();
            return $"_content/ClientUI.Components/icons/{iconName}.png";
        }

        public Dictionary<string, string> GetDatasources() => new()
        { 
            {
                "File", "File"
            }
        };

        public List<string> GetPortfolioTypes()
        {
            return new List<string>()
            {
                PortfolioType.Folder.GetDisplayDescription(),
                PortfolioType.Project.GetDisplayDescription(),
                PortfolioType.Dataset.GetDisplayDescription()
            };
        }

        public async Task SaveFieldMetaData(string id, List<FieldMetaData> fields)
        {
            await _httpClient.PostAsJsonAsync("/api/Datasource/SetFieldMetaData", 
                new CreateFieldMetaDataRequest 
                {
                    Id = id,
                    Fields = fields 
                });
        }

        public async Task<SourceState> GetSourceState(string id)
        {
            var httpResponseMessage = await _httpClient.GetAsync($"/api/Datasource/GetSourceState?id={id}");

            if (!httpResponseMessage.IsSuccessStatusCode || httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }

            return await httpResponseMessage.Content.ReadFromJsonAsync<SourceState>();
        }

        public async Task SaveConfig(string id, SourceConfig sourceConfig)
        {
            await _httpClient.PostAsJsonAsync("/api/Datasource/SetConfigurations", 
                new CreateConfigRequest
                {
                    Id = id,
                    Config = sourceConfig
                });
        }
    }
}