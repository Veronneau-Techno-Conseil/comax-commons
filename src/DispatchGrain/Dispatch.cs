using CommunAxiom.Commons.Client.ClusterEventStream.Extentions;
using CommunAxiom.Commons.Client.Contracts.Grains.Dispatch;
using CommunAxiom.Commons.Client.Grains.DispatchGrain.RuleEngine;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.Grains.DispatchGrain
{
    [ImplicitStreamSubscription(Constants.DefaultNamespace)]
    [AuthorizeClaim]
    [StatelessWorker(1)]
    public class Dispatch : Grain, IDispatch
    {
        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.ImplicitStream);

            var stream = streamProvider.GetEventStream();

            await stream.SubscribeAsync(async (msg, seqToken) =>
            {
                var dispatchRuleEngine = new DispatchRuleEngine(streamProvider);
                
                await dispatchRuleEngine.Process(msg);
            });
            
        }
    }    
}

