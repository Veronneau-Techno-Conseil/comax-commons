
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
        string PROJECT { get; }
        string DATABASE { get; }
        string ID { get; set; }
        string TheType { get; set; }
        string Name { get; set; }
        string ParentId { get; set; }
        Portfolio portfolio { get; set; }
        List<Portfolio> Portfolios { get; set; }
        Task CreatePortfolio(Portfolio portfolio);
        Task<List<Portfolio>?> GetPortfolios();
        Task<bool> CheckIfUnique(string name);
    }
}