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
    public class AgentBusiness : BusinessBase
    {
        private readonly OrchestratorConfig _orchestratorConfig;
        public AgentBusiness(CommonsActorState commonsActorState, OrchestratorConfig orchestratorConfig, IComaxGrainFactory comaxGrainFactory, ISettingsProvider settingsProvider): base(commonsActorState, comaxGrainFactory, settingsProvider)
        {
            _orchestratorConfig = orchestratorConfig;
            
        }

        public override async Task RefreshActor()
        {
            var prop = this.GetOrAdd(PropertyTypes.LastAck, DateTime.MinValue.Ticks.ToString());
            if (IsOverdue(prop.Value, _orchestratorConfig.AckFrequency))
            {
                await this.MessageUser(MessageScopes.MSG_SCOPE_PRIVATE, MessageTypes.OrchestratorInstructions.MSG_TYPE_ACK_ALIVE, "Acknowledge alive", null);
                await this.UpdateProperty(PropertyTypes.LastAck, DateTime.UtcNow.Ticks.ToString());
            }

            prop = _commonsActorState.Properties.FirstOrDefault(x => x.PropertyType == PropertyTypes.ActorType);
            if(prop == null) 
            {
                _commonsActorState.Properties.Add(new PropertyMap { PropertyType = PropertyTypes.ActorType, Value = "AGENT" });
                _shouldSave = true;
            }

            prop = this.GetOrAdd(PropertyTypes.LastPortfolioSync, DateTime.MinValue.Ticks.ToString());
            if(IsOverdue(prop.Value, _orchestratorConfig.PortfolioSyncInterval))
            {
                var lastNotif = this.GetOrAdd(PropertyTypes.LastPortfolioSyncNotification, DateTime.MinValue.Ticks.ToString());
                
                if (IsOverdue(lastNotif.Value, _orchestratorConfig.PortfolioSyncReminderInterval))
                {
                    await this.MessageUser(MessageScopes.MSG_SCOPE_PRIVATE, MessageTypes.OrchestratorInstructions.MSG_TYPE_SYNC_PORTFOLIO, "Sync Portfolios", null);
                    lastNotif.Value = DateTime.UtcNow.Ticks.ToString();
                    _shouldSave = true;
                }
            }

        }


        public override async Task<bool> UpdateProperty(PropertyTypes property, string? value = null)
        {
            var res = await base.UpdateProperty(property, value);
            if (res)
                return res;
            switch(property)
            {
                case PropertyTypes.LastPortfolioSync:
                    this.AddOrUpdate(PropertyTypes.LastPortfolioSync, DateTime.UtcNow.Ticks.ToString());
                    this._commonsActorState.Properties.RemoveAll(x => x.PropertyType == PropertyTypes.LastPortfolioSyncNotification);
                    return true;
            }
            return false;
        }
    }
}
