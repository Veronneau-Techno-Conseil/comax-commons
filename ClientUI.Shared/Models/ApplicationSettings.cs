using CommunAxiom.Commons.Client.Contracts.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public class ApplicationSettings
    {
        public string BaseAddress { get; set; }
        public string UIFramework { get; set; }

        public OIDCSettings OIDC { get; set; }
        public Jwt Jwt { get; set; }
    }
}
