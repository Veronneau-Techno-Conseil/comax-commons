using Comax.Commons.Orchestrator.Contracts.Mailbox;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MailboxGrain
{
    public class Mailbox : Grain, IMailbox
    {
        public Mailbox([PersistentState("accounts")] IPersistentState<MailboxState> mbState)
        {
            //mbState.State
        }
        public Task<bool> HasMail()
        {
            return Task.FromResult(false);
        }

        public override Task OnActivateAsync()
        {
            return Task.CompletedTask;
            //IStreamProvider streamProvider = base.GetStreamProvider("SimpleStreamProvider");
            //IAsyncStream<T> stream = streamProvider.GetStream<SOIEvent>(this.GetPrimaryKeyString(), "MyStreamNamespace");
            //StreamSubscriptionHandle<T> subscription = await stream.SubscribeAsync(IAsyncObserver<T>);
        }
    }
}