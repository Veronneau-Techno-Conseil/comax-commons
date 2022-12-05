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

namespace AgentGrain
{
    public class AgentRepo
    {
        private bool _loaded = false;
        private IPersistentState<AgentState> _persistentState;
        private ThrottleDebounce.RateLimitedFunc<Task> _debouncedSave = null;

        public AgentRepo(IPersistentState<AgentState> persistentState)
        {
            _persistentState = persistentState;
            _debouncedSave = ThrottleDebounce.Throttler.Throttle(ForceSaveState, TimeSpan.FromSeconds(30));
        }
        
        private async Task EnsureLoaded()
        {
            if (!_loaded)
            {
                await _persistentState.ReadStateAsync();
                _loaded = true;
            }
        }

        public async Task AddUserMessage(MailMessage mailMessage)
        {
            await EnsureLoaded();
            var user = this._persistentState.State.UserAuthStates.Where(x => x.PrincipalId == mailMessage.To).FirstOrDefault();
            user.MailMessages.Add(mailMessage);
            await _debouncedSave.Invoke();
        }

        public async Task RemoveMessage(string userUri, Guid message)
        {
            await EnsureLoaded();
            var user = this._persistentState.State.UserAuthStates.Where(x => x.PrincipalId == userUri).FirstOrDefault();
            var msg = user.MailMessages.FirstOrDefault(x => x.MsgId == message);
            if(msg != null)
            {
                user.MailMessages.Remove(msg);
            }
            await _debouncedSave.Invoke();

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

        public async Task ConnectionState(bool isConnected)
        {
            await this.EnsureLoaded();
            _persistentState.State.OrchestratorConnectionState.IsConnected = isConnected;
            this.SaveState();
        }

        public async Task ForceSaveState()
        {
            if (_loaded)
            {
                await _persistentState.WriteStateAsync();
            }
        }

        public void SaveState()
        {
            if (!_loaded)
                return;

            _ = _debouncedSave.Invoke();
        }

        public async Task<OrchestratorConnectionState> GetConnectionState()
        {
            await this.EnsureLoaded();
            return _persistentState.State.OrchestratorConnectionState;
        }

        public async Task<UserAuthState> GetUserAuthState(string v)
        {
            await this.EnsureLoaded();
            var user = _persistentState.State.UserAuthStates.FirstOrDefault(x => x.PrincipalId == v);
            return user.Clone();
        }
    }
}
