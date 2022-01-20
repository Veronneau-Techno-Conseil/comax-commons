using CommunAxiom.Commons.Client.Contracts.Encryption;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Orleans;

namespace CommunAxiom.Commons.Client.Contracts.Account
{
    public interface IAccount: IGrainWithIntegerKey
    {
        Task<string> GetAccountName();
        Task SetAccountName(string name);
        Task<bool> IsAuthenticated();
        Task<Stream> EncryptStream(Stream data);
        Task<string> TestGrain(string Grain);

    }
}
