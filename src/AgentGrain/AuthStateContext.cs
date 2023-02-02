using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain
{
    public static class AuthStateContext
    {
        public static AsyncLocal<AuthState> CurrentAuthState { get; } = new AsyncLocal<AuthState>();

        public struct AuthState
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
