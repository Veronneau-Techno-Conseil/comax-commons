using Orleans.Security.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator
{
    internal class Security
    {
        public static void ConfigurePolicyOptions(AuthorizationOptions options)
        {

            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
        }
    }
}
