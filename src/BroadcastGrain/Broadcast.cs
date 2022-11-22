using CommunAxiom.Commons.Client.Contracts.Broadcast;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class Broadcast : Grain, IBroadcast
    {
        private BroadcastRulesEngine _broadcastRulesEngine;

        public override Task OnActivateAsync()
        {
            _broadcastRulesEngine = new BroadcastRulesEngine(this.GetStreamProvider(Constants.ImplicitStream));

            return base.OnActivateAsync();
        }

        public Task Notify(Message message)
        {
            return _broadcastRulesEngine.Process(message);
        }
    }
}