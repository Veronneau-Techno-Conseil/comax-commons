using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.RulesEngine.Mock
{
    public class MessageMock
    {
        public string From { get; set; }
        public string To { get; set; }
        public string FromOwner { get; set; }
        public string Scope { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
    }
}
