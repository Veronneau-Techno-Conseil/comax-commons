using CommunAxiom.Commons.Client.Contracts.Account;
using System;
using System.IO;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class Accounts : Grain, IAccount
    {
        private readonly IPersistentState<AccountDetails> _actDetails;
        
        public Accounts([PersistentState("accounts")]IPersistentState<AccountDetails> actDetails)
        {
            _actDetails = actDetails;
        }

        public async Task<AccountState> CheckState(string clientIdRef = null)
        {
            await _actDetails.ReadStateAsync();
            if(_actDetails.State != null && !string.IsNullOrWhiteSpace(_actDetails.State.AccountsToken))
            {
                if (!string.IsNullOrWhiteSpace(clientIdRef) && _actDetails.State.ClientID != clientIdRef)
                    return AccountState.ClientMismatch;
                return AccountState.CredentialsSet;
            }
            

            return AccountState.Initial;
        }

        public Task<Stream> EncryptStream(Stream data)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountDetails> GetDetails()
        {
            await this._actDetails.ReadStateAsync();
            return this._actDetails.State;
        }

        public Task Initialize(AccountDetails accountDetails)
        {
            this._actDetails.State = accountDetails;
            return this._actDetails.WriteStateAsync();
        }
    }
}
