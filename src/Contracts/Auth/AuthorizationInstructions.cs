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
        public Instruction Step { get; set; }
        /// <summary>
        /// Payload
        /// </summary>
        public string Payload { get; set; }
    }
}
