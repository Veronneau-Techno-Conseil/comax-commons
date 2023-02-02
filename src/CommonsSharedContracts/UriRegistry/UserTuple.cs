using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry
{
    public class UserTuple
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Uri { get; set; }
        public Guid InternalId { get; set; }
    }
}
