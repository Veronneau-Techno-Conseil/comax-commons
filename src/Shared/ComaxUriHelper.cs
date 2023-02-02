using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunAxiom.Commons.Shared
{
    public class ComaxUriHelper
    {
        private static Regex UriRegex = new Regex("^(?<userType>\\w+)://(?<user>[a-z0-9A-Z\\-]+)/(?<resource>[a-zA-Z0-9\\/\\-]+)");

        public static bool TryParseUri(string uri, out UriDescriptor? result)
        {
            if (!UriRegex.IsMatch(uri))
            {
                result = null;
                return false;
            }

            var match = UriRegex.Match(uri);
            result = new UriDescriptor();
            result.AccountType = match.Groups["userType"].Value;
            result.AccountId = match.Groups["user"].Value;

            if (match.Groups.Any(g => g.Name == "resource"))
            {
                result.Resource = match.Groups["resource"].Value;
            }
            return true;
        }

        public class UriDescriptor
        {
            public string AccountType { get; set; }
            public string AccountId { get; set; }
            public string Resource { get; set; }

            public string AccountUri
            {
                get
                {
                    return $"{AccountType}://{AccountId}";
                }
            }
        }
    }
}
