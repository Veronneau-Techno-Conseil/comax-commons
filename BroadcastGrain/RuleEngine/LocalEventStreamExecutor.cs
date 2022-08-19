using CommunAxiom.Commons.Shared.RuleEngine;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class LocalEventStreamExecutor : IExecutor<Message>
    {
        public Task Execute(Message param)
        {
            // Accessing event stream
            // Pushing event stream
            ExecutorTargets.LocalTarget = param;
            return Task.CompletedTask;
        }
    }
}
