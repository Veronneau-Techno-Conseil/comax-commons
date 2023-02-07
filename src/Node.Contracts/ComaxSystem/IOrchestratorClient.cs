using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.Central;
using CommunAxiom.Commons.Shared.RuleEngine;
using Comax.Commons.Orchestrator.Contracts.SOI;
using Orleans;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using Comax.Commons.Orchestrator.Contracts.PublicBoard;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using Comax.Commons.Orchestrator.Contracts.CommonsActor;
using Comax.Commons.Orchestrator.Contracts.Portfolio;
using CommunAxiom.Commons.CommonsShared.Contracts.DataSeed;
using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;

namespace Comax.Commons.Orchestrator.Contracts.ComaxSystem
{
    public interface IOrchestratorClient : IDisposable
    {
        IUriRegistry GetUriRegistry(string id = "");
        IPublicBoard GetPublicBoard();
        Task<IEventMailboxClient> GetEventMailbox(Guid? id = null);
        
        Task<ICommonsActor> GetActor(string? uri = null);

        ICentral GetCentral();
        Task<ISubjectOfInterest> GetSubjectOfInterest();
        IClusterClient ClusterClient { get; }
        IPortfolioRegistry GetPortfolioRegistry();
        IDataSeed GetDataSeed(Guid id);
        IAsyncStream<DataChunkObject> GetDataChunkStream(Guid id);
        Task Close();
    }
}
