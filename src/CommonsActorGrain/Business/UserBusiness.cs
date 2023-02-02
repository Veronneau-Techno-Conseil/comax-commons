using Comax.Commons.Orchestrator.Contracts;
using Comax.Commons.Orchestrator.Contracts.CommonsActor;
using Comax.Commons.Orchestrator.Contracts.SOI;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.CommonsActorGrain.Business
{
    public class UserBusiness : BusinessBase
    {
        private readonly OrchestratorConfig _orchestratorConfig;
        public UserBusiness(CommonsActorState commonsActorState, OrchestratorConfig orchestratorConfig, IComaxGrainFactory comaxGrainFactory, ISettingsProvider settingsProvider): 
            base(commonsActorState, comaxGrainFactory, settingsProvider)
        {
            _orchestratorConfig = orchestratorConfig;
        }

        public override async Task RefreshActor()
        {
            await this.MessageUser(MessageScopes.MSG_SCOPE_PRIVATE, MessageTypes.OrchestratorInstructions.MSG_TYPE_ACK_ALIVE, "Acknowledge alive", null);

            var prop = _commonsActorState.Properties.FirstOrDefault(x => x.PropertyType == PropertyTypes.ActorType);
            if(prop == null) 
            {
                _commonsActorState.Properties.Add(new PropertyMap { PropertyType = PropertyTypes.ActorType, Value = "USER" });
                _shouldSave = true;
            }
        }

        public override async Task<bool> UpdateProperty(PropertyTypes property, string? value = null)
        {
            var res = await base.UpdateProperty(property, value);
            if (res)
                return res;
            
            return false;
        }
    }
}
