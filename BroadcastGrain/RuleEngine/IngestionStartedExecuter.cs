using CommunAxiom.Commons.Shared.RuleEngine;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class IngestionStartedExecutor : IExecutor<Message>
    {
        public Task Execute(Message param)
        {
            ExecutorTargets.LocalTarget = param;
            return Task.CompletedTask;
        }
    }
}
