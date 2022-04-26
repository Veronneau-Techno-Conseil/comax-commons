using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Auth
{
    public class AuthorizationInstructions
    {
        /// <summary>
        /// Current authentication step
        /// </summary>
        public AuthStep Step { get; set; }
        /// <summary>
        /// Url for authorization
        /// </summary>
        public string OpenUrl { get; set; }
        /// <summary>
        /// Returned security token
        /// </summary>
        public string SecurityToken { get; set; }
        /// <summary>
        /// Returned identity token
        /// </summary>
        public string IdentityToken { get; set; }
    }
}
