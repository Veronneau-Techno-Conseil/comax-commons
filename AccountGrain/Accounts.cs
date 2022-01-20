using CommunAxiom.Commons.Client.Contracts.Account;
using System;
using System.IO;
using System.Threading.Tasks;
using Orleans;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class Accounts : Grain, IAccount
    {
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

        public Task<string> TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}
