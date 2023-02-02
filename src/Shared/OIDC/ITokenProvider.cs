using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared.OIDC
{
    public interface ITokenProvider
    {
        Task<string?> FetchToken();
    }
}
