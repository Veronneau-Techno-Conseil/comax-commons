using System;

namespace CommunAxiom.Commons.ClientUI.ApiContracts
{
    public class AuthStart
    {
        public string ClientId { get; set; }
    }
    public class AuthSteps
    {
        public const string OK = "OK";
        public const string LOGIN = "Login";
        public const string Reset = "Reset";
        public const string ApiSecret = "ApiSecret";
        public const string AuthApi = "AuthApi";

        public const string ERR_ClientMismatch = "CLIDMISMATCH";
        public const string ERR_Authentication = "AUTH_ERR";
    }

    public class TokenData
    {

        public virtual string AccessToken
        {
            get;
            internal set;
        }

        public virtual string IdentityToken
        {
            get;
            internal set;
        }

  
        public virtual string RefreshToken
        {
            get;
            internal set;
        }

        public virtual DateTime AccessTokenExpiration
        {
            get;
            internal set;
        }

        public virtual DateTime AuthenticationTime
        {
            get;
            internal set;
        }

    }
}