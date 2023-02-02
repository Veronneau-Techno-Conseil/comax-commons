using Comax.Commons.Orchestrator.Contracts.CommonsActor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.CommonsActorGrain.Business
{
    public interface IActorBO
    {
        Task RefreshActor();
        bool ShouldSave();
        Task<bool> UpdateProperty(PropertyTypes property, string? value = null);
        Task<string?> GetProperty(PropertyTypes property);
        CommonsActorState GetState();
    }
}
