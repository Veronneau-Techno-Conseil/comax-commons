using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Auth
{
    public enum AuthStep
    {
        PendingAuthorization = 0,
        TokenReturned= 1
    }
}
