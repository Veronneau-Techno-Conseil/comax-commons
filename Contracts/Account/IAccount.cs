using CommunAxiom.Commons.Client.Contracts.Encryption;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Orleans;

namespace CommunAxiom.Commons.Client.Contracts.Account
{
    public interface IAccount: IGrainWithStringKey
    {
        Task<string> GetAccountName();
        Task SetAccountName(string name);
        Task<bool> IsAuthenticated();
        Task<Stream> EncryptStream(Stream data);
        //Added to test the launching the grains. To be removed then
        Task<string> TestGrain(string GrainId);
        Task<string> SetDetails(string GrainId, AccountDetails account);
        Task<string> GetDetails(string GrainId);
    }
}
