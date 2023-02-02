using CommunAxiom.Commons.Client.AgentClusterRuntime.Extentions;
using CommunAxiom.Commons.Client.Contracts.Grains.Dispatch;
using CommunAxiom.Commons.Client.Grains.DispatchGrain.RuleEngine;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.OIDC;
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.Grains.DispatchGrain
{
    [ImplicitStreamSubscription(CommunAxiom.Commons.Orleans.OrleansConstants.StreamNamespaces.DefaultNamespace)]
    //[AuthorizeClaim]
    //[StatelessWorker(1)]
    public class Dispatch : Grain, IDispatch
    {
        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(OrleansConstants.Streams.ImplicitStream);
            var key = this.GetPrimaryKey();
            var stream = streamProvider.GetStream<Message>(
                    key, OrleansConstants.StreamNamespaces.DefaultNamespace);
            

            var gf = new Orleans.GrainFactory(this.GrainFactory, this.GetStreamProvider);
            await stream.SubscribeAsync(async (msg, seqToken) =>
            {
                var dispatchRuleEngine = new DispatchRuleEngine(streamProvider, gf);
                
                await dispatchRuleEngine.Process(msg);
            });
            
        }
    }    
}

