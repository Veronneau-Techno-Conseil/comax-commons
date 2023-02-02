using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunAxiom.Commons.Shared.RulesEngine
{
    public static class MessageHelper
    {
        public static Regex USER_ID_REGEX = new Regex("^usr://(?<id>[{(]?[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?)");
        public static string? GetUserId(string url)
        {
            var m = USER_ID_REGEX.Match(url);
            if(!m.Groups.Any(x=>x.Name == "id"))
                return null;
            return m.Groups["id"].Value;
        }

        public static string UserUri(string id)
        {
            return $"usr://{id}";
        }

        public static Regex URI_REGEX = new Regex("^(?<entity>\\w+)://(?<id>[0-9A-Za-z\\-]+)");
        public static string? GetEntityType(string url)
        {
            var m = URI_REGEX.Match(url);

            return m.Groups.FirstOrDefault(x=>x.Name == "entity")?.Value;
        }

        public static string? GetEntityId(string url)
        {
            var m = URI_REGEX.Match(url);

            return m.Groups.FirstOrDefault(x => x.Name == "id")?.Value;
        }
    }
}
