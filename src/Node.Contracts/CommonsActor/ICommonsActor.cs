using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.CommonsActor
{
    public interface ICommonsActor: IGrainWithStringKey
    {
        Task<ActorTypes> GetActorType();
        Task SetPortfolioSynchronized();
        Task SetAlive();

    }
}
