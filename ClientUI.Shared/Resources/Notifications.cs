using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Resources
{
    public class Notifications
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        public Criticality Criticality { get; set; } //0 info, 1 warning, 2 sucess, 3 critical
    }

    public enum Criticality
    {
        Info = 0,
        Warning = 1,
        Success = 2,
        Critical = 3
    }
}
