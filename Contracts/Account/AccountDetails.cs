using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Account
{
    public class AccountDetails
    {
        public static string AccessToken { get; set; }
        public Guid Identifier { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime CreationDate { get; set; }

        public string ApplicationId { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string AccountsToken { get; set; }
    }
}
