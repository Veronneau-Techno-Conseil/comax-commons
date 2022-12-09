using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using IdentityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public static class UserAuthStateExtensions
    {
        public static UserAuthState Clone(this UserAuthState userAuthState)
        {
            return new UserAuthState
            {
                PrincipalId = userAuthState.PrincipalId,
                IsAuthorised = userAuthState.IsAuthorised,
                Subscription = userAuthState.Subscription
            };
        }
    }
}
