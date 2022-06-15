using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Client.Contracts.Portfolio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Services.Portfolios
{
    public class PortfolioService: IPortfolioService
    {
        private readonly ICommonsClientFactory _clusterClient;

        public PortfolioService(ICommonsClientFactory clusterClient)
        {
            _clusterClient = clusterClient;
        }

        public async Task<Portfolio> GetPortfolioDetails(string portfolioID)
        {
            var PortfolioDetails = await _clusterClient.WithClusterClient(async cc =>
            {
                return await cc.GetPortfolio("Portfolios").GetAPortfolioDetails(portfolioID);
            });

            return PortfolioDetails;
        }

        public async Task<PortfoliosList> GetAllPortfolios()
        {
            //if not created, create the PortfoliosList
            var state = await _clusterClient.WithClusterClient(async cc =>
            {
                var portfolio = cc.GetPortfolio("Portfolios");
                return await portfolio.ListIsSet();
            });

            if (state == false)
            {
                await _clusterClient.WithClusterClient(async cc =>
                {
                    var createList = await cc.GetPortfolio("Portfolios").CreatePortfoliosList();
                });
            }

            //Get the updated List
            var PortfoliosFinalList = await _clusterClient.WithClusterClient(async cc =>
            {
                return await cc.GetPortfolio("Portfolios").GetListDetails();
            });

            return PortfoliosFinalList;
        }

        public async Task CreatePortfolio(Portfolio portfolio)
        {
            //if not created, create the PortfoliosList
            var state = await _clusterClient.WithClusterClient(async cc =>
            {
                var existingPortfolio = cc.GetPortfolio("Portfolios");
                return await existingPortfolio.ListIsSet();
            });

            if (state == false)
            {
                await _clusterClient.WithClusterClient(async cc =>
                {
                    var createList = await cc.GetPortfolio("Portfolios").CreatePortfoliosList();
                });
            }

            Portfolio newPortfolio = new Portfolio
            {
                ID = portfolio.ID,
                Name = portfolio.Name,
                TheType = portfolio.TheType,
                ParentId = portfolio.ParentId
            };

            //Add the portfolio created to the existing portfolioList
            if (newPortfolio != null)
            {
                await _clusterClient.WithClusterClient(async cc =>
                {
                    var portfolioCreated = await cc.GetPortfolio("Portfolios").AddAPortfolio(newPortfolio);
                });
            }
        }

        public string JustTest()
        {
            return "A";
        }
    }
}
