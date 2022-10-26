using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.ComaxSystem;
using CommunAxiom.Commons.Shared.Configuration;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.SiloShared.Conf
{
    public class OIDCConfig
    {
        public static OIDCSettings Config
        {
            get
            {
                return Instance.Config;
            }
            set
            {
                Instance.Config = value;
            }
        }

        private static class Instance
        {
            private static object _lockObj = new object();
            private static OIDCSettings oidcConfig;
            public static OIDCSettings Config
            {
                get
                {
                    return oidcConfig;
                }
                set
                {
                    lock (_lockObj)
                    {
                        oidcConfig = value;
                    }
                }
            }
        }
    }

    public static class OIDCExtensions
    {
        public static async Task SetCredentials(this ICommonsClusterClient commonsClusterClient, IConfiguration configuration)
        {
            var actGrain = commonsClusterClient.GetAccount();
            var act = await actGrain.GetDetails();
            OIDCSettings authSettings = new OIDCSettings();
            configuration.Bind(Sections.OIDCSection, authSettings);
            Conf.OIDCConfig.Config = authSettings;
        }
    }
}
