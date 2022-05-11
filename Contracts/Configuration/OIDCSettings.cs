using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Configuration
{
    public class OIDCSettings
    {
        public string Authority { get; set; }
        public string Scopes { get; set; }
    }
}
