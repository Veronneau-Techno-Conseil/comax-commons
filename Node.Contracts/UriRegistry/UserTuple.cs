using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.UriRegistry
{
    public class UserTuple
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public Guid InternalId { get; set; }
    }
}
