using Comax.Commons.Orchestrator.Contracts;
using Comax.Commons.Orchestrator.Contracts.CommonsActor;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.CommonsActorGrain.Business
{
    public abstract class BusinessBase: IActorBO, IUserContextAccessor
    {
        protected bool _shouldSave;
        protected readonly CommonsActorState _commonsActorState;
        protected readonly IComaxGrainFactory _comaxGrainFactory;
        protected readonly ISettingsProvider _settingsProvider;

        protected BusinessBase(CommonsActorState commonsActorState, IComaxGrainFactory comaxGrainFactory, ISettingsProvider settingsProvider) 
        { 
            _commonsActorState = commonsActorState ?? new CommonsActorState();
            if(_commonsActorState.Properties == null)
            {
                _commonsActorState.Properties = new List<PropertyMap>();
            }
            _comaxGrainFactory = comaxGrainFactory;
            _settingsProvider = settingsProvider;
        }

        public CommonsActorState GetState()
        {
            return _commonsActorState;
        }

        public abstract Task RefreshActor();

        public bool ShouldSave()
        {
            return _shouldSave;
        }
        public virtual async Task<bool> UpdateProperty(PropertyTypes property, string? value = null)
        {
            switch(property)
            {
                case PropertyTypes.LastSeen:
                    this.AddOrUpdate(PropertyTypes.LastSeen, DateTime.UtcNow.Ticks.ToString());
                    return true;
            }
            return false;
        }

        public virtual async Task<string?> GetProperty(PropertyTypes property)
        {
            var prop = _commonsActorState.Properties.FirstOrDefault(x => x.PropertyType == property);
            if (prop == null)
                return null;

            return prop.Value;
        }

        protected void AddOrUpdate(PropertyTypes property, string? value = null)
        {
            var prop = _commonsActorState.Properties.FirstOrDefault(x => x.PropertyType == property);
            if (prop == null)
            {
                prop = new PropertyMap { PropertyType = property };
                _commonsActorState.Properties.Add(prop);
            }
            prop.Value = value;
        }

        protected PropertyMap GetOrAdd(PropertyTypes property, string? defaultValue = null)
        {
            var prop = _commonsActorState.Properties.FirstOrDefault(x => x.PropertyType == property);
            if (prop == null)
            {
                prop = new PropertyMap { PropertyType = property, Value = defaultValue };
                _commonsActorState.Properties.Add(prop);
                _shouldSave = true;
            }
            return prop;
        }

        protected bool IsOverdue(string ticks, int seconds)
        {
            var date = new DateTime(long.Parse(ticks), DateTimeKind.Utc);
            return (DateTime.UtcNow - date) > TimeSpan.FromSeconds(seconds);
        }

        protected async Task MessageUser(string scope, string msgType, string subject, string body)
        {
            var soi = await _comaxGrainFactory.GetSubjectOfInterest();
            var settings = await _settingsProvider.GetOIDCSettings();
            await soi.Broadcast(new CommunAxiom.Commons.Shared.RuleEngine.Message
            {
                CreatedDate = DateTime.UtcNow,
                From = $"orch://{settings.ClientId}",
                FromOwner = $"orch://{settings.ClientId}",
                Id = Guid.NewGuid(),
                Scope = scope,
                Subject = subject,
                To = this.GetUser().GetUri(),
                Type = msgType,
                Payload = body
            });
        }
    }
}
