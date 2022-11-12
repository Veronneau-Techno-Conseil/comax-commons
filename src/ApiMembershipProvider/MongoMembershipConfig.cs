using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.ApiMembershipProvider
{
    public class MongoMembershipConfig
    {
        public string authDb { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string[] hosts { get; set; }
        public int port { get; set; }
        public bool directConnection { get; set; }
        public bool useTls { get; set; }
        public bool allowInsecureTls { get; set; }
    }
}
