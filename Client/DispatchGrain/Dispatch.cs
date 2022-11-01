using Comax.Commons.Orchestrator.EventMailboxGrain;
using CommunAxiom.Commons.Client.Contracts.Grains.Dispatch;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;

namespace CommunAxiom.Commons.Client.Grains.DispatchGrain
{
    [ImplicitStreamSubscription(EventMailboxConstants.MAILBOX_STREAM_NS)]
    [AuthorizeClaim(ClaimType = "https://orchestrator.communaxiom.org/mailbox")]
    public class Dispatch : Grain, IDispatch
    {
        private readonly EventMailboxBusiness _eventMailboxBusiness;
        public override async Task OnActivateAsync()
        {
            _eventMailboxBusiness = new EventMailboxBusiness(this.GetStreamProvider(Constants.DefaultStream));
            _eventMailboxBusiness.Init(_storageState);

            var streamProvider = GetStreamProvider(Constants.DefaultNamespace);
            var key = this.GetPrimaryKey();
            var stream = streamProvider.GetEventStream();//GetStream<Message>(key, EventMailboxConstants.MAILBOX_STREAM_NS);

            await stream.SubscribeAsync(async (msg, seqToken) =>
            {
                // Pipe through business rule engine
                // await _eventMailboxBusiness.SendMail(mm);
            });
            
        }
    }    
}

