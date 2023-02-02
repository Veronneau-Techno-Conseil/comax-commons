using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.PublicBoard
{
    public interface IPublicBoardObserver : IGrainObserver
    {
        void NewPost(Message message); 
    }
}
