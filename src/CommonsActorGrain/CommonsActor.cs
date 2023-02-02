using Comax.Commons.Orchestrator.CommonsActorGrain.Business;
using Comax.Commons.Orchestrator.Contracts;
using Comax.Commons.Orchestrator.Contracts.CommonsActor;
using Comax.Commons.Orchestrator.Contracts.PublicBoard;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.RulesEngine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.CommonsActorGrain
{
    [AuthorizeClaim]
    public class CommonsActor : Grain, ICommonsActor
    {
        private readonly OrchestratorConfig _orchestratorConfig;
        private readonly ILogger<CommonsActor> _logger;

        private readonly IPersistentState<CommonsActorState> _storageState;
        private readonly ISettingsProvider _settingsProvider;

        private IActorBO _actorBo;

        public CommonsActor(ILogger<CommonsActor> logger, IOptions<OrchestratorConfig> orchestratorConfigOptions, ISettingsProvider settingsProvider, [PersistentState("storageGrain")] IPersistentState<CommonsActorState> storageState) 
        {
            _logger = logger;
            _storageState = storageState;
            _orchestratorConfig = orchestratorConfigOptions.Value;
            _settingsProvider = settingsProvider;
        }

        public override async Task OnActivateAsync()
        {
            var at = await GetActorType();
            switch(at)
            {
                case ActorTypes.Agent:
                    await _storageState.ReadStateAsync();
                    _actorBo = new AgentBusiness(_storageState.State, _orchestratorConfig, new GrainFactory(this.GrainFactory, this.GetStreamProvider), _settingsProvider);
                    break;
                case ActorTypes.User:
                    _actorBo = new UserBusiness(_storageState.State, _orchestratorConfig, new GrainFactory(this.GrainFactory, this.GetStreamProvider), _settingsProvider);
                    break;
            }
        }

        public async Task<ActorTypes> GetActorType()
        {
            var type = MessageHelper.GetEntityType(this.GetPrimaryKeyString());
            switch (type.ToLower())
            {
                case "com":
                    return ActorTypes.Agent;
                case "usr":
                    return ActorTypes.User;
                default:
                    return ActorTypes.Unknown;
            }
        }

        public async Task IAmAlive()
        {
            if (_actorBo == null)
                return;
            await _actorBo.RefreshActor();
            if (_actorBo.ShouldSave())
            {
                _storageState.State = _actorBo.GetState();
                await _storageState.WriteStateAsync();
            }
        }

        public async Task<bool> UpdateProperty(PropertyTypes property, string? value = null)
        {
            if (_actorBo == null)
                return false;

            return await _actorBo.UpdateProperty(property, value);
        }

        public async Task<string?> GetProperty(PropertyTypes property)
        {
            return await _actorBo.GetProperty(property);
        }
    }
}
