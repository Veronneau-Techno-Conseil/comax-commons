using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using IdentityModel.Client;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain
{
    public class AgentRepo
    {
        private bool _loaded = false;
        private bool _shouldSave = false;
        private IPersistentState<AgentState> _persistentState;
        private ThrottleDebounce.RateLimitedFunc<Task> _debouncedSave = null;

        public AgentRepo(IPersistentState<AgentState> persistentState)
        {
            _persistentState = persistentState;
            _debouncedSave = ThrottleDebounce.Throttler.Throttle(SetSave, TimeSpan.FromSeconds(30));
        }
        
        private async Task EnsureLoaded()
        {
            if (!_loaded)
            {
                await _persistentState.ReadStateAsync();
                _loaded = true;
            }
        }

        public async Task EnsureSaved()
        {
            if(_shouldSave && _loaded)
            {
                await _persistentState.WriteStateAsync();
                _shouldSave = false;
            }
        }

        public async Task SetUserAuthState(UserAuthState userAuthState)
        {
            await this.EnsureLoaded();
            var user = _persistentState.State.UserAuthStates.FirstOrDefault(x => x.PrincipalId == userAuthState.PrincipalId);
            if (user == null)
            {
                user = userAuthState;
            }
            else
            {
                _persistentState.State.UserAuthStates.Remove(user);
                user = userAuthState;
                _persistentState.State.UserAuthStates.Add(user);
            }
            this.SaveState();
        }

        
        private Task SetSave()
        {
            if (_loaded)
            {
                _shouldSave = true;
            }
            return Task.CompletedTask;
        }

        public void SaveState()
        {
            if (!_loaded)
                return;

            _ = _debouncedSave.Invoke();
        }


        public async Task<UserAuthState> GetUserAuthState(string v)
        {
            await this.EnsureLoaded();
            var user = _persistentState.State.UserAuthStates.FirstOrDefault(x => x.PrincipalId == v);
            if(user == null)
            {
                user = new UserAuthState
                {
                    PrincipalId = v
                };
                _persistentState.State.UserAuthStates.Add(user);
                _ = _debouncedSave.Invoke();
            }
            return user.Clone();
        }

    }
}
