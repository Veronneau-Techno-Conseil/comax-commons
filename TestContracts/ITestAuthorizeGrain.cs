using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestContracts
{
    public interface ITestAuthorizeGrain
    {
        Task<bool> TestOpen();
        Task<bool> TestClaimExists();
        Task<bool> TestClaimEquals();
        Task<bool> TestClaimStarts();
        Task<bool> TestClaimContains();
        Task<bool> TestClaimRegex();
        Task<bool> TestAuthenticated();

    }
}
