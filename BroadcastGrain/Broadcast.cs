using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Client.Contracts.Broadcast;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class Broadcast : Grain, IBroadcast
    {
        private readonly BroadcastRulesEngine _broadcastRulesEngine;

        public Broadcast()
        {
            _broadcastRulesEngine = new BroadcastRulesEngine(this.GetStreamProvider(Constants.DefaultStream));
        }

        public Task Notify(Message message)
        {
            return _broadcastRulesEngine.Process(message);
        }
    }

}
