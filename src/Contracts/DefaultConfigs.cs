using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public static class DefaultConfigs
    {
        public static Dictionary<string, string> Configs { get; private set; }
        static DefaultConfigs()
        {
            Configs = new Dictionary<string, string>()
            {
                {"ClusterId", "0.0.1-a1"},
                {"ServiceId", "CommonsClientCluster"},
                {"siloPort", "7717"},
                {"gatewayPort", "30000" }
            };
        }
    }
}