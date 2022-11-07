using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Shared.OIDC
{
    public interface ITokenProvider
    {
        Task<string?> FetchToken();
    }
}
