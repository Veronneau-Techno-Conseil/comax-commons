using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared.OIDC
{
    public class OIDCSettings
    {
        public string Authority { get; set; }
        public string Scopes { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }
}
