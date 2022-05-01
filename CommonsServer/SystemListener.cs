using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Silo.System;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Silo
{
    public class SystemListener
    {
        private readonly ClusterManagement clusterManagement;
        private StreamSubscriptionHandle<SystemEvent> streamSubscriptionHandle;

        public SystemListener(ClusterManagement clusterManagement)
        {
            this.clusterManagement = clusterManagement;
        }

        public async Task Listen()
        {

            streamSubscriptionHandle = await Services.CommonsClusterClient.SubscribeSystem(OnsystemEvent, OnSystemStreamError, OnCompleted);
        }

        public async Task OnsystemEvent(SystemEvent systemEvent, StreamSequenceToken token)
        {
            switch (systemEvent.Type)
            {
                case SystemEventType.ClusterAuthenticationComplete:
                    await clusterManagement.SetSilo(Silos.Main);
                    break;
                case SystemEventType.ClusterAuthenticationReset:
                    await clusterManagement.SetSilo(Silos.Pilot);
                    break;
            }
        }
        
        public Task OnSystemStreamError(Exception e)
        {
            Console.Error.WriteLine(e.ToString());
            Console.Error.WriteLine(e.StackTrace);
            return Task.CompletedTask;
        }
        
        public Task OnCompleted()
        {
            return Task.CompletedTask;
        }
    }
}
