
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.TreeView;

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
    }
}