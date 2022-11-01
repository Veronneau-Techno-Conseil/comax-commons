using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.Models
{
    public enum AccessLevel
    {
        NoAccess=0,
        Read=1<<0,
        Access=1<<0,
        Write=1<<1,
        Delete=1<<2,
        Manage=1<<3,
        All=Read|Write|Delete|Manage
    }
}
