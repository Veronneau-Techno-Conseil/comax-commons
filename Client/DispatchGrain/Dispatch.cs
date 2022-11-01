using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using Comax.Commons.Orchestrator.EventMailboxGrain;
using CommunAxiom.Commons.Client.ClusterEventStream.Extentions;
using CommunAxiom.Commons.Client.Contracts.Grains.Dispatch;
using CommunAxiom.Commons.Client.Grains.DispatchGrain.RuleEngine;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using Orleans;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.Grains.DispatchGrain
{
    [ImplicitStreamSubscription(EventMailboxConstants.MAILBOX_STREAM_NS)]
    [AuthorizeClaim(ClaimType = "https://orchestrator.communaxiom.org/mailbox")]
    public class Dispatch : Grain, IDispatch
    {
        private EventMailboxBusiness _eventMailboxBusiness;
 
        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.DefaultStream);
            var grainFactory = new GrainFactory(GrainFactory);
            
            _eventMailboxBusiness = new EventMailboxBusiness(streamProvider, grainFactory);

            var stream = streamProvider.GetEventStream();

            await stream.SubscribeAsync(async (msg, seqToken) =>
            {
                var dispatchRuleEngine = new DispatchRuleEngine(streamProvider);
                
                await dispatchRuleEngine.Process(msg);

                await _eventMailboxBusiness.DropMail(new MailMessage
                {
                    From = msg.From,
                    MsgId = msg.Id,
                    ReceivedDate = DateTime.UtcNow,
                    Subject = msg.Subject,
                    Type = msg.Type
                });
            });
            
        }
    }    
}

