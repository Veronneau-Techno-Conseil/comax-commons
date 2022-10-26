using CommunAxiom.Commons.Client.Contracts.Account;
using System;
using System.IO;
using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using CommunAxiom.Commons.Client.Contracts;
using Microsoft.Extensions.Configuration;
using CommunAxiom.Commons.Orleans.Security;
using System.Linq;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class Accounts : Grain, IAccount
    {
        private readonly IConfiguration _configuration;
        private readonly AccountBusiness _accountBusiness;
        public Accounts(IConfiguration configuration, AccountBusiness accountBusiness, [PersistentState("accounts")] IPersistentState<AccountDetails> actDetails)
        {
            _accountBusiness = accountBusiness;
            _accountBusiness.Init(actDetails);
            _configuration = configuration;
        }

        public async Task<AccountState> CheckState(bool forceRefresh, string clientIdRef = null)
        {
            return await this._accountBusiness.CheckState(forceRefresh, clientIdRef);
        }

        public Task<Stream> EncryptStream(Stream data)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountDetails> GetDetails()
        {
            return await this._accountBusiness.GetDetails();
        }

        public async Task Initialize(AccountDetails accountDetails)
        {
            await this._accountBusiness.Initialize(accountDetails);
        }

        public Task<string> SecurityCheck()
        {
            var user = this.GetUser();
            var ci = user.Identities.First();
            
            return Task.FromResult(ci.Name);
        }
    }
}
