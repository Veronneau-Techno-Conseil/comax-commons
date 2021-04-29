using CommunAxiom.Commons.Client.Contracts.Account;
using CommunAxiom.Commons.Client.Contracts.Encryption;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain
{
    public class Accounts : Orleans.Grain, IAccount
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
    }
}
