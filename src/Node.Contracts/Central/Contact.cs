using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.Orchestrator.Contracts.Central
{
    public class Contact
    {
        public string Id { get; set; }
        public string Uri
        {
            get
            {
                return $"usr://{Id}";
            }
        }
        public string UserName { get; set; }
    }
}
