using CommunAxiom.Commons.Shared.RuleEngine;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class NewDataVersionExecutor : IExecutor<Message>
    {
        public Task Execute(Message param)
        {
            ExecutorTargets.LocalTarget = param;
            return Task.CompletedTask;
        }
    }
}
