
using CommunAxiom.Commons.Shared.Configuration;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.Configuration;

namespace CommunAxiom.Commons.Shared.OIDC
{
    public static class OIDC
    {
        public static OIDCSettings GetOIDCConfig(this IConfiguration configuration, string section)
        {
            OIDCSettings authSettings = new OIDCSettings();
            configuration.Bind(section, authSettings);
            return authSettings;
        }
    }
}
