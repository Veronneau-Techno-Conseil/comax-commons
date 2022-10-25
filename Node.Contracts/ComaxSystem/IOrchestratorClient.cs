using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Comax.Commons.Orchestrator.Contracts.UriRegistry;
using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.Central;
using CommunAxiom.Commons.Shared.RuleEngine;
using Comax.Commons.Orchestrator.Contracts.SOI;

namespace Comax.Commons.Orchestrator.Contracts.ComaxSystem
{
    public interface IOrchestratorClient: IDisposable
    {
        Task<Message> GetMail(Guid id);
        IUriRegistry GetUriRegistry(string id = "");
        IPublicBoard GetPublicBoard();
        Task<IEventMailbox> GetEventMailbox(Guid? id = null);
        ICentral GetCentral();
        Task<ISubjectOfInterest> GetSubjectOfInterest();
        Task Close();
    }
}
