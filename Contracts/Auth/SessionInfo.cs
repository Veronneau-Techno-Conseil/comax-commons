using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Auth
{
    public class SessionInfo
    {
        public virtual byte[] UserData { get; set; }
        public string AccessToken { get; set; }
        public string IdentityToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset? AuthenticationTime { get; set; }
        public DateTimeOffset AuthenticationExpiration { get; set; }
    }
}
