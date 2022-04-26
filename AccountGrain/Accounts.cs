using CommunAxiom.Commons.Client.Contracts.Account;
using System;
using System.IO;
using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class Accounts : Grain, IAccount
    {

        private AccountDetails _accountDetails = new AccountDetails();

        public Task<Stream> EncryptStream(Stream data)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAccountName()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAuthenticated()
        {
            throw new NotImplementedException();
        }

        public Task SetAccountName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<string> TestGrain(string GrainId)
        {
            return Task.FromResult($"The Account Grain with Id: {GrainId} has been launched. Check it on the dashboard");
        }

        Task<string> IAccount.SetDetails(string GrainId, AccountDetails account)
        {

            _accountDetails.ApplicationId = account.ApplicationId;
            _accountDetails.ClientID = account.ClientID;
            _accountDetails.ClientSecret = account.ClientSecret;
            _accountDetails.AccountsToken = account.AccountsToken;

            return Task.FromResult("The ClientID, Secret and AccessToken has been passed to grain " + GrainId + " successfully.\n You ClientID is: " + _accountDetails.ClientID + "\nYour ClientSecret is: " + _accountDetails.ClientSecret + "\nYour AccessToken is: " + _accountDetails.AccountsToken);
        }

        public Task<string> GetDetails(string GrainId)
        {
            return Task.FromResult("The Client Details handled by the Grain " + GrainId + " are: \nClientID: " + _accountDetails.ClientID + "\nClientSecret: " + _accountDetails.ClientSecret + "\nAccessToken: " + _accountDetails.AccountsToken);
        }
    }
}
