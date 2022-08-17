using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.RulesEngine.Mock
{
    public static class ExecutorTargets
    {
        public static MessageMock LocalTarget { get; set; }
        public static MessageMock PublicTarget { get; set; }

        public static void Flush()
        {
            LocalTarget = null;
            PublicTarget = null;
        }
    }
}
