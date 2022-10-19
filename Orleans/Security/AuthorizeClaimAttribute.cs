using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Orleans.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method,
        AllowMultiple = true, Inherited = true)]
    public class AuthorizeClaimAttribute : Attribute
    {
        public AuthorizeClaimAttribute()
        {
        }
        public string ClaimType { get; set; }
        public string? ClaimValueFilter { get; set;}
        public FilterMode MatchStrategy { get; set; } = FilterMode.Equals;

        public enum FilterMode
        {
            Equals,
            Contains,
            Regex,
            StartsWith
        }
    }
}
