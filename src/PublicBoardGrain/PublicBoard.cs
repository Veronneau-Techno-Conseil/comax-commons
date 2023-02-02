using Comax.Commons.Orchestrator.Contracts;
using Comax.Commons.Orchestrator.Contracts.PublicBoard;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MailboxGrain
{
    [AuthorizeClaim]
    public class PublicBoard : Grain, IPublicBoard
    {
        private readonly ObserverManager<IPublicBoardObserver> _subsManager;
        private readonly ILogger _logger;
        private readonly PublicBoardBusiness _publicBoardBusiness;

        public PublicBoard(ILogger logger, [PersistentState("publicBoard")] IPersistentState<PublicBoardIndex> storageState)
        {
            PublicBoardRepo publicBoardRepo = new PublicBoardRepo(storageState);
            GrainFactory grainFactory = new GrainFactory(this.GrainFactory, this.GetStreamProvider);
            _publicBoardBusiness = new PublicBoardBusiness(publicBoardRepo, grainFactory, logger);
            _subsManager = new ObserverManager<IPublicBoardObserver>(TimeSpan.FromMinutes(5), logger, "subs");
            _logger = logger;
        }

        public Task Broadcast(Message message)
        {
            return Task.CompletedTask;
        }

        public Task Subscribe(IPublicBoardObserver publicBoardObserver, Guid latestMsg)
        {
            _subsManager.Subscribe(publicBoardObserver, publicBoardObserver);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(IPublicBoardObserver publicBoardObserver)
        {
            _subsManager.Unsubscribe(publicBoardObserver);
            return Task.CompletedTask;
        }
    }
}
