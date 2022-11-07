using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Orleans.Security
{
    public class AccessControl
    {
        private static IEnumerable<AuthorizeClaimAttribute> GetAttributes(IIncomingGrainCallContext grainCallContext)
        {
            var atts = new List<AuthorizeClaimAttribute>();
            var authorizeClaimAttributes =
                grainCallContext.InterfaceMethod.GetCustomAttributes<AuthorizeClaimAttribute>();

            if (authorizeClaimAttributes != null && authorizeClaimAttributes.Count() > 0)
                atts.AddRange(authorizeClaimAttributes);

            authorizeClaimAttributes =
                grainCallContext.ImplementationMethod.GetCustomAttributes<AuthorizeClaimAttribute>();

            if (authorizeClaimAttributes != null && authorizeClaimAttributes.Count() > 0)
                atts.AddRange(authorizeClaimAttributes);

            authorizeClaimAttributes =
                grainCallContext.InterfaceMethod.DeclaringType.GetCustomAttributes<AuthorizeClaimAttribute>();

            if (authorizeClaimAttributes != null && authorizeClaimAttributes.Count() > 0)
                atts.AddRange(authorizeClaimAttributes);

            authorizeClaimAttributes =
               grainCallContext.ImplementationMethod.DeclaringType.GetCustomAttributes<AuthorizeClaimAttribute>();

            if (authorizeClaimAttributes != null && authorizeClaimAttributes.Count() > 0)
                atts.AddRange(authorizeClaimAttributes);

            return atts;
        }

        public static bool ShouldAuthorize(IIncomingGrainCallContext grainCallContext)
        {
            var atts = GetAttributes(grainCallContext);

            return atts.Count() > 0;
        }
        public static AccessControlValidationResult IsAuthorized(IIncomingGrainCallContext grainCallContext, ClaimsPrincipal? claimsPrincipal)
        {
            var authorizeClaimAttributes = GetAttributes(grainCallContext);

            if (authorizeClaimAttributes == null || authorizeClaimAttributes.Count() == 0)
                return new AccessControlValidationResult { IsAuthorized = true, IsAuthenticated = claimsPrincipal == null ? false : true };

            if (claimsPrincipal == null)
                return new AccessControlValidationResult { IsAuthorized = false, IsAuthenticated = false };

            var res = new AccessControlValidationResult() { IsAuthenticated = true, IsAuthorized = true };

            bool claimCheck = false;

            foreach (var authorizeClaimAttribute in authorizeClaimAttributes)
            {
                if(string.IsNullOrWhiteSpace(authorizeClaimAttribute.ClaimType))
                    continue;

                if (string.IsNullOrWhiteSpace(authorizeClaimAttribute.ClaimValueFilter)) {
                    claimCheck = claimsPrincipal.HasClaim(x => x.Type == authorizeClaimAttribute.ClaimType);
                    res.IsAuthorized &= claimCheck;
                    if(!claimCheck)
                        res.FailedClaimTypes.Add((authorizeClaimAttribute.ClaimType, "REQUIRED"));
                    continue;
                }

                switch (authorizeClaimAttribute.MatchStrategy)
                {
                    case AuthorizeClaimAttribute.FilterMode.Equals:
                        claimCheck = claimsPrincipal.HasClaim(x => x.Type == authorizeClaimAttribute.ClaimType &&
                            authorizeClaimAttribute.ClaimValueFilter.Equals(x.Value, StringComparison.InvariantCultureIgnoreCase));
                        res.IsAuthorized &= claimCheck;
                        if (!claimCheck)
                            res.FailedClaimTypes.Add((authorizeClaimAttribute.ClaimType, $"VALUE({authorizeClaimAttribute.ClaimValueFilter})"));
                        continue;
                    case AuthorizeClaimAttribute.FilterMode.Contains:
                        claimCheck = claimsPrincipal.HasClaim(x => x.Type == authorizeClaimAttribute.ClaimType &&
                            authorizeClaimAttribute.ClaimValueFilter.Contains(x.Value, StringComparison.InvariantCultureIgnoreCase));
                        res.IsAuthorized &= claimCheck;
                        if (!claimCheck)
                            res.FailedClaimTypes.Add((authorizeClaimAttribute.ClaimType, $"CONTAINS({authorizeClaimAttribute.ClaimValueFilter})"));
                        continue;
                    case AuthorizeClaimAttribute.FilterMode.StartsWith:
                        claimCheck = claimsPrincipal.HasClaim(x => x.Type == authorizeClaimAttribute.ClaimType &&
                            authorizeClaimAttribute.ClaimValueFilter.StartsWith(x.Value, StringComparison.InvariantCultureIgnoreCase));
                        res.IsAuthorized &= claimCheck;
                        if (!claimCheck)
                            res.FailedClaimTypes.Add((authorizeClaimAttribute.ClaimType, $"STARTSWITH({authorizeClaimAttribute.ClaimValueFilter})"));
                        continue;
                    case AuthorizeClaimAttribute.FilterMode.Regex:
                        var regex = new Regex(authorizeClaimAttribute.ClaimValueFilter);
                        claimCheck = claimsPrincipal.HasClaim(x => x.Type == authorizeClaimAttribute.ClaimType &&
                            regex.IsMatch(x.Value));
                        res.IsAuthorized &= claimCheck;
                        if (!claimCheck)
                            res.FailedClaimTypes.Add((authorizeClaimAttribute.ClaimType, $"REGEX({authorizeClaimAttribute.ClaimValueFilter})"));
                        continue;
                    default:
                        throw new InvalidOperationException("AuthorizeClaimAttribute.FilterMode value does not exist");
                }
            }
            return res;
        }
    }
}
