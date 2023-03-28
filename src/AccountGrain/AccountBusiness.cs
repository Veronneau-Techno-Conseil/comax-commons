using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.DotnetSdk.Helpers.OIDC;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class AccountBusiness
    {
        private readonly IConfiguration _configuration;
        private readonly ISettingsProvider _settingsProvider;
        private AccountRepo _accountRepo;
        public AccountBusiness(IConfiguration configuration, ISettingsProvider settingsProvider)
        {
            this._configuration = configuration;
            this._settingsProvider = settingsProvider;
        }

        public void Init(IPersistentState<AccountDetails> actDetails)
        {
            _accountRepo = new AccountRepo(actDetails);
        }

        public async Task<AccountDetails> GetDetails()
        {
            return await this._accountRepo.Fetch();
        }

        public async Task<AccountState> CheckState(bool forceRefresh, string clientIdRef = null)
        {
            var details = await _accountRepo.Fetch();

            if (details != null && !string.IsNullOrWhiteSpace(details.ClientID))
            {
                if (!string.IsNullOrWhiteSpace(clientIdRef) && details.ClientID != clientIdRef)
                    return AccountState.ClientMismatch;

                //Throttle force refresh
                if ((forceRefresh && (details.LastRefresh == null || details.LastRefresh?.ToUniversalTime().AddMinutes(2) < DateTime.UtcNow)) || DateTime.UtcNow > details.NextRefresh?.ToUniversalTime())
                {
                    await this.UpdateClientCredentials(details);
                }

                return details.State;
            }

            return AccountState.Initial;
        }

        public async Task Initialize(AccountDetails accountDetails)
        {
            if (!string.IsNullOrWhiteSpace(accountDetails.ClientID))
            {
                await this.UpdateClientCredentials(accountDetails);
            }
            else
            {
                await this._accountRepo.Update(accountDetails);
            }
        }

        private async Task UpdateClientCredentials(AccountDetails details)
        {
            //todo: Complete token maintenance / status
            TokenClient tokenClient = new TokenClient(_configuration);
            var settings = await _settingsProvider.GetOIDCSettings();

            var (isSuccess, data) = await tokenClient.AuthenticateClient(TokenClient.SCOPES_OFFLINE, details.ClientID, settings.Secret);
            if (!isSuccess)
            {
                details.State = AccountState.AuthenticationError;
                details.AccessToken = string.Empty;
                details.RefreshToken = string.Empty;
                //5 minutes before token expiration
                details.NextRefresh = DateTime.UtcNow.AddSeconds(300);
            }
            else
            {
                var principal = await tokenClient.RequestIntrospection(details.ClientID, settings.Secret, data.access_token);
                details.ApplicationUri = principal.Item2.GetUri();
                details.State = AccountState.Initial;
                details.AccessToken = data.access_token;
                details.RefreshToken = data.refresh_token;
                //5 minutes before token expiration
                details.NextRefresh = DateTime.UtcNow.AddSeconds(data.expires_in - 300);
            }
            details.LastRefresh = DateTime.UtcNow;
            await _accountRepo.Update(details);
        }
    }
}
