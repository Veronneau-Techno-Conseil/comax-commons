using CommunAxiom.Commons.Client.Contracts.Encryption;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Orleans;
using CommunAxiom.Commons.Orleans.Security;

namespace CommunAxiom.Commons.Client.Contracts.Account
{
    public interface IAccount: IGrainWithGuidKey
    {
        Task Initialize(AccountDetails accountDetails);
        Task<Stream> EncryptStream(Stream data);
        Task<AccountDetails> GetDetails();
        Task<AccountState> CheckState(bool forceRefresh, string clientIdRef = null);

        [AuthorizeClaim]
        Task<string> SecurityCheck();
    }
}
