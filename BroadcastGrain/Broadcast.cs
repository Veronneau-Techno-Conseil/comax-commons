using CommunAxiom.Commons.Client.Contracts.Broadcast;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using System;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class Broadcast : Grain, IBroadcast
    {
        private readonly BroadcastRulesEngine engine;

        public Broadcast(IServiceProvider serviceProvider)
        {
            engine = new BroadcastRulesEngine(serviceProvider);
        }

        public Task Notify(Message message)
        {
            return engine.Process(message);
        }
    }

}
