using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.SiloShared.System
{
    public interface IClusterManagement
    {
        Silos CurrentSilo { get; }
        Task Heartbeat();
        Task SetSilo(Silos requiredSilo);
        Task<bool> IsServiceAuthSet();
    }
}
