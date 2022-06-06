using CommunAxiom.Commons.Client.Contracts.Account;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class AccountRepo
    {
        private readonly IPersistentState<AccountDetails> actDetails;
        public AccountRepo(IPersistentState<AccountDetails> actDetails)
        {
            this.actDetails = actDetails;
        }

        public async Task<AccountDetails> Fetch()
        {
            await actDetails.ReadStateAsync();
            return actDetails.State;
        }

        public async Task Update(AccountDetails accountDetails)
        {
            actDetails.State = accountDetails;
            await actDetails.WriteStateAsync();
        }
    }
}
