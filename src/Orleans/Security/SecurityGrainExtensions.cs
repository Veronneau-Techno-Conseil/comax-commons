﻿using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CommunAxiom.Commons.Orleans.Security
{
    public static class SecurityGrainExtensions
    {
        public static ClaimsPrincipal GetUser(this Grain grain)
        {
            var o = RequestContext.Get("cp"); 
            if(o == null)
                return null;
            var cc = (ClaimsContainer)o;
            var cp = cc.GetPrincipal();
            return cp;
        }

        public static ClaimsPrincipal GetUser(this IUserContextAccessor instance)
        {
            var o = RequestContext.Get("cp");
            if (o == null)
                return null;
            var cc = (ClaimsContainer)o;
            var cp = cc.GetPrincipal();
            return cp;
        }

        public static string GetUri(this ClaimsPrincipal cp)
        {
            return cp.FindFirst(Constants.URI_CLAIM).Value;
        }
    }
}
