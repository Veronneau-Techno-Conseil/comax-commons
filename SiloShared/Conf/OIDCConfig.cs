using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Configuration;
using Microsoft.Extensions.Configuration;
using Orleans.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.SiloShared.Conf
{
    public class OIDCConfig
    {
        public IdentityServer4Info Server { get; set; }

        public static OIDCConfig Config
        {
            get
            {
                return Instance.Config;
            }
        }

        private static class Instance
        {
            private static object _lockObj = new object();
            private static OIDCConfig identityServer4Config;
            public static OIDCConfig Config 
            { 
                get
                {
                    lock (_lockObj)
                    {
                        return identityServer4Config = identityServer4Config ?? new OIDCConfig();
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
            Conf.OIDCConfig.Config.Server = new IdentityServer4Info(authSettings.Authority, act.ClientID, act.ClientSecret, authSettings.Scopes);
        }
    }
}
