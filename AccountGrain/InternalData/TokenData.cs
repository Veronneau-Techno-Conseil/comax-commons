using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Grains.AccountGrain.InternalData
{
    public class TokenData
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }

    }
}
