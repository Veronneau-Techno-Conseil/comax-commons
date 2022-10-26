using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Orleans.Security
{
    public class AccessControlException: Exception
    {
        public AccessControlException(): base()
        {

        }

        public bool IsAuthenticated { get; set; }
        public List<(string,string)>? FailedClaimTypes { get; set; }

    }
}
