using CommunAxiom.Commons.Client.Contracts.Account;
using System;
using System.IO;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using CommunAxiom.Commons.Client.Contracts;
using Microsoft.Extensions.Configuration;
using CommunAxiom.Commons.Client.Grains.AccountGrain.InternalData;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class Accounts : Grain, IAccount
    {
        const string SCOPES = "openid offline_access";
        private readonly IPersistentState<AccountDetails> _actDetails;
        private readonly IConfiguration _configuration;
        public Accounts(IConfiguration configuration, [PersistentState("accounts")] IPersistentState<AccountDetails> actDetails)
        {
            _actDetails = actDetails;
            _configuration = configuration;
        }

        public async Task<AccountState> CheckState(bool forceRefresh, string clientIdRef = null)
        {
            await _actDetails.ReadStateAsync();

            if (_actDetails.State != null && !string.IsNullOrWhiteSpace(_actDetails.State.ClientID))
            {
                if (!string.IsNullOrWhiteSpace(clientIdRef) && _actDetails.State.ClientID != clientIdRef)
                    return AccountState.ClientMismatch;

                //Throttle force refresh
                if ((forceRefresh && (_actDetails.State.LastRefresh == null || _actDetails.State.LastRefresh?.AddMinutes(2) < DateTime.Now)) || DateTime.UtcNow > _actDetails.State.NextRefresh)
                {
                    await this.UpdateClientCredentials();
                }

                return _actDetails.State.State;
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

        public async Task Initialize(AccountDetails accountDetails)
        {
            this._actDetails.State = accountDetails;

            if (!string.IsNullOrWhiteSpace(accountDetails.ClientID) && !string.IsNullOrWhiteSpace(accountDetails.ClientSecret)) 
            {
                await this.UpdateClientCredentials();
            }
            
            await this._actDetails.WriteStateAsync();

        }

        private async Task UpdateClientCredentials()
        {
            //todo: Complete token maintenance / status
            TokenClient tokenClient = new TokenClient(_configuration);
            await _actDetails.ReadStateAsync();
            var (isSuccess, data) = await tokenClient.GetToken(_actDetails.State.ClientID, _actDetails.State.ClientSecret, SCOPES);
            if (!isSuccess)
            {
                _actDetails.State.State = AccountState.AuthenticationError;
                _actDetails.State.AccessToken = string.Empty;
                _actDetails.State.RefreshToken = string.Empty;
                //5 minutes before token expiration
                _actDetails.State.NextRefresh = DateTime.UtcNow.AddSeconds(300);
            }
            else
            {
                _actDetails.State.State = AccountState.CredentialsSet;
                _actDetails.State.AccessToken = data.access_token;
                _actDetails.State.RefreshToken = data.refresh_token;
                //5 minutes before token expiration
                _actDetails.State.NextRefresh = DateTime.UtcNow.AddSeconds(data.expires_in - 300);
            }
            _actDetails.State.LastRefresh = DateTime.UtcNow;
            await _actDetails.WriteStateAsync();
        }
    }
}
