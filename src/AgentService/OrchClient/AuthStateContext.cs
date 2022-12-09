using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public static class AuthStateContext
    {
        internal static AsyncLocal<AuthState> CurrentAuthState { get; } = new AsyncLocal<AuthState>();

        internal struct AuthState
        {
            public string Token { get; set; }
            public bool IsEmpty { get
                {
                    return string.IsNullOrWhiteSpace(Token);
                } 
            }

            public static AuthState Default { get; } = new AuthState();
        }
    }
}
