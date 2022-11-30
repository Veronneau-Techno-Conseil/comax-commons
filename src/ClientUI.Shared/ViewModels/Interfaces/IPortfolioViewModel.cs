using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Client.Contracts.IO;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces
{
    public interface IPortfolioViewModel
    {
        Task CreatePortfolio(PortfolioModel portfolio);
        Task<IList<PortfolioModel>> GetPortfolios();

        Task<bool> CheckIfUnique(string name);

        string GetIcon(PortfolioType portfolioType);

        Dictionary<string, string> GetDatasources();
        
        List<string> GetPortfolioTypes();

        Task SaveFieldMetaData(string id, List<FieldMetaData> fields);
        Task<SourceState> GetSourceState(string id);
        Task SaveConfig(string id, SourceConfig sourceConfig);
    }
}