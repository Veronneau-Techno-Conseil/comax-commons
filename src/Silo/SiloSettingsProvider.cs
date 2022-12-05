using CommunAxiom.Commons.Shared.OIDC;
using Orleans;
using O = Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Client.Contracts.Account;
using Microsoft.Extensions.Configuration;

namespace CommunAxiom.Commons.Client.Silo
{
    //public class SiloSettingsProvider : ISettingsProvider
    //{
    //    private readonly O.IGrainFactory _grainFactory;
    //    private readonly IConfiguration _configuration;
    //    public SiloSettingsProvider(O.IGrainFactory grainFactory, IConfiguration configuration)
    //    {
    //        _grainFactory = grainFactory;
    //        _configuration = configuration;
    //    }

    //    public async Task<OIDCSettings> GetOIDCSettings()
    //    {
    //        var acnt = _grainFactory.GetGrain<IAccount>(Guid.Empty);

    //        var details = await acnt.GetDetails();

    //        var settings = new OIDCSettings();

    //        _configuration.Bind("OIDC", settings);

    //        settings.ClientId = details.ClientID;
    //        settings.Secret = details.ClientSecret;
            
    //        return settings;
    //    }
    //}
}
