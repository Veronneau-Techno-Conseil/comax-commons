
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces
{
    public interface IPortfolioViewModel
    {
        Task CreatePortfolio(PortfolioModel portfolio);
        Task<IEnumerable<PortfolioTreeViewItem>> GetPortfolios();

        Task<bool> CheckIfUnique(string name);

        string GetIcon(PortfolioType portfolioType);

        List<string> GetDatasources();

        List<string> GetPortfolioTypes();
    }
}