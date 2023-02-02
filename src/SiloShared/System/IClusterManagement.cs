using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.SiloShared.System
{
    public interface IClusterManagement
    {
        public bool SiloStarted { get; }
        Task Heartbeat();
        Task StartSilo();
        
    }
}
