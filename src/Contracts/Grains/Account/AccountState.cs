using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Account
{
    public enum AccountState
    {
        Initial = 0,
        AuthenticationError = 2,
        ClientMismatch = 3
    }
}
