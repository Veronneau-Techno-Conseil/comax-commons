using CommunAxiom.Commons.Client.Contracts.Grains.Portfolio;
using CommunAxiom.Commons.CommonsShared.Contracts;
using CommunAxiom.Commons.Orleans;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.AgentClusterRuntime;
using Microsoft.Extensions.Logging;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.RulesEngine;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;

namespace PortfolioGrain
{
    [ImplicitStreamSubscription(OrleansConstants.StreamNamespaces.PORTFOLIO_NOTIFS_NS)]
    public class Portfolio : Grain, IPortfolio
    {
        private PortfolioBusiness _portfolioBusiness;
        private readonly IPersistentState<CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.PortfolioItem> _portDetails;
        private readonly IPersistentState<PortfoliosList> _portList;
        private StreamSubscriptionHandle<MailMessage> _fetchOperation;
        public Portfolio([PersistentState("portfolios")] IPersistentState<CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.PortfolioItem> portDetails,
            [PersistentState("portfoliosList")] IPersistentState<PortfoliosList> portList)
        {
            _portDetails = portDetails;
            _portList= portList;
        }

        public override async Task OnActivateAsync()
        {
            _portfolioBusiness = new PortfolioBusiness(ServiceProvider);
            _portfolioBusiness.Init(_portDetails, _portList);
            var streamProvider = GetStreamProvider(OrleansConstants.Streams.ImplicitStream);
            var key = this.GetPrimaryKey();
            var stream = streamProvider.GetStream<ClusterNotification>(
                    key, OrleansConstants.StreamNamespaces.PORTFOLIO_NOTIFS_NS);

            await stream.SubscribeAsync(async (msg, seqToken) =>
            {
                if(msg.Verb == ClusterNotification.NEW_MAIL_VERB)
                {
                    await FetchMail();
                }                
            });

            await base.OnActivateAsync();
        }

        protected string UriSuffix
        {
            get
            {
                return "portfolio";
            }
        }

        public async Task FetchMail()
        {
            try
            {
                var logger = this.ServiceProvider.GetService<ILogger<Portfolio>>();

                var cu = await this.ServiceProvider.GetComaxUri(UriSuffix);
                var au = await this.ServiceProvider.GetAgentUri();

                var gf = this.GrainFactory.AsComax(this.GetStreamProvider);
                var mb = await gf.GetEventMailbox(logger, cu);
                var hm = await mb.HasMail();
                if(hm)
                {
                    _fetchOperation = await mb.GetStream(
                        async (msg, squence) => 
                        {
                            switch (msg.Type)
                            {
                                case MessageTypes.OrchestratorInstructions.MSG_TYPE_SYNC_PORTFOLIO:
                                    await _portfolioBusiness.PushToOrchestrator(au);
                                    await mb.DeleteMail(msg.MsgId);
                                    break;
                            }
                        }, (exc) =>
                        {
                            return Task.CompletedTask;
                        }, () => {
                            if (_fetchOperation != null)
                            {
                                try
                                {
                                    _fetchOperation.UnsubscribeAsync();
                                }
                                catch { }
                            }
                            return Task.CompletedTask;
                        });
                   
                }

                
                //TODO support fetch mail to portfolio instructions to sync to orchestrator
                await Task.CompletedTask;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task AddPortfolio(PortfolioItem portfolio)
        {
            await _portfolioBusiness.AddPortfolio(portfolio);
        }

        public async Task<IEnumerable<PortfolioItem>> GetPortfoliosList()
        {
            return await _portfolioBusiness.GetList();
        }

        public async Task<IEnumerable<PortfolioItem>> FilterPortfolios(string filter)
        {
            return await _portfolioBusiness.FilterPortfolios(filter);
        }

        public async Task<PortfolioItem> GetAPortfolioDetails(Guid portfolioID)
        {
            return await _portfolioBusiness.GetAPortfolioDetails(portfolioID);
        }

        public async Task<bool> CheckIfUnique(string name)
        {
            return await _portfolioBusiness.CheckIfUnique(name);
        }
    }
}