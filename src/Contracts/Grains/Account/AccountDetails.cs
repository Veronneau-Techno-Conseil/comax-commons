using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Account
{
    public class AccountDetails
    {
        public string ApplicationId { get; set; }
        public string DisplayName { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? NextRefresh { get; set; }
        public DateTime? LastRefresh { get; set; }
        public AccountState State { get; set; }
    }
}
