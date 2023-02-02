using CommunAxiom.Commons.Client.AgentClusterRuntime;
using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.Orleans.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioGrain
{
    public class PortfolioBusiness : IUserContextAccessor
    {
        private readonly IConfiguration _configuration;
        private PortfolioRepo _portfolioRepo;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        public PortfolioBusiness(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configuration = _serviceProvider.GetService<IConfiguration>();
            _logger = _serviceProvider.GetService<ILogger<PortfolioBusiness>>();
        }

        public void Init(IPersistentState<PortfolioItem> portDetails, IPersistentState<PortfoliosList> portList)
        {
            _portfolioRepo = new PortfolioRepo(portDetails, portList);
        }

        public async Task AddPortfolio(PortfolioItem portfolio)
        {
            // Add check reference parent/child 
            if (portfolio != null)
            {
                if (portfolio.ID == Guid.Empty)
                    throw new InvalidOperationException("Cannot create portfolio entry with zero guid");

                var owner = this.GetUser().GetOwner();
                portfolio.Uri = $"{owner}/portfolio/{portfolio.ID}";
                portfolio.OwnerUri = owner;
                await _portfolioRepo.AddPortfolio(portfolio);
            }
        }

        public async Task<IEnumerable<PortfolioItem>> GetList()
        {
            return await _portfolioRepo.GetList();
        }

        public async Task<IEnumerable<PortfolioItem>> FilterPortfolios(string filter)
        {
            var portfoliosList = await _portfolioRepo.FilterPortfolios(filter);
            return portfoliosList;
        }

        public async Task<PortfolioItem> GetAPortfolioDetails(Guid portfolioID)
        {
            var portfolio = await _portfolioRepo.GetDetails(portfolioID);

            return portfolio;
        }

        public async Task<bool> CheckIfUnique(string name)
        {
            if (name != null)
            {
                return await _portfolioRepo.CheckIfUnique(name);
            }
            return false;
        }

        public async Task PushToOrchestrator(string uri)
        {
            _logger.Info("Logging...");
            var f = _serviceProvider.SegregatedOrchClientFactory();
            await f.SetDefaultToken();

            var owner = this.GetUser().GetOwner();
            var lst = await _portfolioRepo.GetList();

            //TODO sync portfolio data
            await f.WithClusterClient(async cl =>
            {
                var pf = cl.GetPortfolioRegistry();
                var index = await pf.GetIndex();
                if (index.IsError)
                    throw new Exception(index.Error + " " + index.Detail);
                var ixlst = index.Result ?? new List<Comax.Commons.Orchestrator.Contracts.Portfolio.Models.PortfolioItem>();

                //upsert
                foreach (var item in lst)
                {
                    await pf.UpsertPortfolioItem(new Comax.Commons.Orchestrator.Contracts.Portfolio.Models.PortfolioItem
                    {
                        ItemType = Convert(item.Type),
                        Name = item.Name,
                        Description = item.Description,
                        ParentId = item.ParentId,
                        Id = item.ID,
                        PortfolioUri = $"{owner}/portfolio",
                        Uri = item.Uri
                    });
                }

                //delete
                foreach (var i in ixlst)
                {
                    if (!lst.Any(x => x.Uri == i.Uri))
                        await pf.DeletePortfolioItem(i.Uri);
                }

                var actor = await cl.GetActor(uri);
                await actor.UpdateProperty(Comax.Commons.Orchestrator.Contracts.CommonsActor.PropertyTypes.LastPortfolioSync);
            });
        }

        private Comax.Commons.Orchestrator.Contracts.Portfolio.Models.ItemTypes Convert(PortfolioType portfolioType)
        {
            switch (portfolioType)
            {
                case PortfolioType.Project:
                    return Comax.Commons.Orchestrator.Contracts.Portfolio.Models.ItemTypes.Project;
                case PortfolioType.Dataset:
                    return Comax.Commons.Orchestrator.Contracts.Portfolio.Models.ItemTypes.Datasource;
                case PortfolioType.Folder:
                    return Comax.Commons.Orchestrator.Contracts.Portfolio.Models.ItemTypes.Folder;
                default:
                    throw new InvalidOperationException("PortfolioType note supported for conversion");
            }
        }
    }
}