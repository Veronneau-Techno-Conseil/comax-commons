using CommunAxiom.Commons.Client.Contracts.Encryption;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Account
{
    public interface IAccount : Orleans.IGrain
    {
        Task<string> GetAccountName();
        Task SetAccountName(string name);
        Task<bool> IsAuthenticated();
        //X509Certificate Certificate { get; }
        //IEncryptionService EncryptionService { get; }

        Task<Stream> EncryptStream(Stream data);

    }
}
