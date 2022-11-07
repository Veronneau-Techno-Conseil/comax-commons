using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Orleans.Security
{
    public class AccessControlValidationResult
    {

        public bool IsAuthorized { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<(string, string)> FailedClaimTypes { get; set; } = new List<(string, string)>();

    }
}
