using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.AgentGrain.SyncWorker
{
    public static class AuthStateContext
    {
        internal static AsyncLocal<AuthState> State { get; } = new AsyncLocal<AuthState>();

        public struct AuthState : IEquatable<AuthState>
        {
            public string Token { get; set; }
            public bool IsEmpty
            {
                get
                {
                    return string.IsNullOrWhiteSpace(Token);
                }
            }

            public static AuthState Default { get; } = new AuthState();

            public bool Equals(AuthState other)
            {
                return Token == other.Token;
            }
        }
    }
}
