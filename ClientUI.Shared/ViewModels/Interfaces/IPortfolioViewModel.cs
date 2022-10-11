using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces
{
    public interface IPortfolioViewModel
    {
        Task CreatePortfolio(PortfolioModel portfolio);
        Task<IList<PortfolioModel>> GetPortfolios();

        Task<bool> CheckIfUnique(string name);

        string GetIcon(PortfolioType portfolioType);

        List<string> GetDatasources();

        List<string> GetPortfolioTypes();

        Task SaveFieldMetaData(string id, List<FieldMetaData> fields);
        Task<List<FieldMetaData>> GetFieldMetaData(string id);
        Task SaveConfig(string id, SourceConfig sourceConfig);
    }
}