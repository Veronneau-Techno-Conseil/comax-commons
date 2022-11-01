using System;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class AuthStart
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }

    public class AuthResult
    {
        public string? Token { get; set; }        
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
        public const string ERR_Unexpected = "UNEXPECTED";
    }

    public class TokenData
    {

        public string? AccessToken
        {
            get;
            set;
        }

        public string? IdentityToken
        {
            get;
            set;
        }


        public string? RefreshToken
        {
            get;
            set;
        }

        public DateTime AccessTokenExpiration
        {
            get;
            set;
        }

        public DateTime AuthenticationTime
        {
            get;
            set;
        }

    }
}